using CarSpecAPI.Data.Models.DataExtractionModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;

namespace CarsSpecAPI.Services;

public class ImportBatchService : IImportBatchService
{
    private readonly CarsDbContext carsDbContext;

    public ImportBatchService(CarsDbContext context)
    {
        carsDbContext = context;
    }

    // ============================================================
    // GET BATCHES
    // ============================================================

    public async Task<PaginatedImportBatchResponseDto> GetImportBatchesAsync(
        int pageNumber,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        // Intentionally fixed at 10.
        pageSize = 10;

        var query = carsDbContext.ImportBatches
            .AsNoTracking();

        var totalRecords =
            await query.CountAsync(cancellationToken);

        var totalPages =
            totalRecords == 0
                ? 0
                : (int)Math.Ceiling(
                    totalRecords / (double)pageSize);

        if (totalPages > 0 && pageNumber > totalPages)
        {
            return new PaginatedImportBatchResponseDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = false,
                Batches = new List<ImportBatchListDto>()
            };
        }

        var batches = await query
            .OrderByDescending(x => x.StartedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ImportBatchListDto
            {
                ImportBatchId = x.ImportBatchId,
                StartedAt = x.StartedAt,
                CompletedAt = x.CompletedAt,
                Status = x.Status,
                ImportedBy = x.ImportedBy,
                Notes = x.Notes,
                AIModel = x.Aimodel,
                AIProcessedAt = x.AiprocessedAt,

                HasAIResult =
                    x.AiresultJson != null &&
                    x.AiresultJson != "",

                HasFinalResult =
                    x.FinalJson != null &&
                    x.FinalJson != "",

                Documents = x.ImportDocuments
                    .Select(d => new ImportBatchDocumentDto
                    {
                        ImportDocumentId = d.ImportDocumentId,
                        DocumentType = d.DocumentType,
                        DocumentName = d.DocumentName,
                        FileSizeBytes = d.FileSizeBytes
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new PaginatedImportBatchResponseDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages,
            Batches = batches
        };
    }

    // ============================================================
    // STAGE BATCH
    // ============================================================

    public async Task<StageBatchResultDto> StageBatchAsync(
        int importBatchId,
        CancellationToken cancellationToken = default)
    {
        if (importBatchId <= 0)
        {
            throw new ArgumentException(
                "Import batch ID must be greater than zero.",
                nameof(importBatchId));
        }

        await using IDbContextTransaction transaction =
            await carsDbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable,
                cancellationToken);

        try
        {
            var batch =
                await carsDbContext.ImportBatches
                    .FirstOrDefaultAsync(
                        x => x.ImportBatchId == importBatchId,
                        cancellationToken);

            if (batch == null)
            {
                throw new InvalidOperationException(
                    $"Import batch {importBatchId} was not found.");
            }

            if (!string.Equals(
                    batch.Status,
                    "Reviewed&Merged",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Batch {importBatchId} cannot be staged because its " +
                    $"current status is '{batch.Status}'. " +
                    "Only 'Reviewed&Merged' batches can be staged.");
            }

            if (string.IsNullOrWhiteSpace(batch.FinalJson))
            {
                throw new InvalidOperationException(
                    $"Batch {importBatchId} does not contain FinalJson.");
            }

            BrochureExtractionDto finalJson;

            try
            {
                finalJson =
                    JsonSerializer.Deserialize<BrochureExtractionDto>(
                        batch.FinalJson,
                        JsonOptions())
                    ?? throw new InvalidOperationException(
                        "FinalJson deserialized to null.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"FinalJson for batch {importBatchId} is not valid JSON.",
                    ex);
            }

            var validation =
                await ValidateFinalJsonAsync(
                    batch,
                    finalJson,
                    cancellationToken);

            if (!validation.IsValid)
            {
                throw new ImportBatchValidationException(
                    importBatchId,
                    validation.Errors);
            }

            var result = new StageBatchResultDto
            {
                ImportBatchId = importBatchId
            };

            // ========================================================
            // 1. ROOT IMPORT RECORD
            // ========================================================

            var modelRecord = new ImportRecord
            {
                ImportBatchId = batch.ImportBatchId,

                ImportDocumentId =
                    await GetPrimaryDocumentIdAsync(
                        batch.ImportBatchId,
                        cancellationToken),

                EntityType = "Model",
                EntityKey = finalJson.Vehicle!.ModelName,
                FieldName = "Model",
                ExtractedValue = finalJson.Vehicle.ModelName,
                NormalizedValue = finalJson.Vehicle.ModelName,

                Status = "Staged",
                ReviewStatus = "Reviewed",
                SourceId = batch.DataSourceId
            };

            carsDbContext.ImportRecords.Add(modelRecord);

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 2. IMPORT MODEL
            // ========================================================

            var importModel = new ImportModel
            {
                ImportRecordId = modelRecord.ImportRecordId,

                BrandName =
                    finalJson.Vehicle.Brand!.Trim(),

                ModelName =
                    finalJson.Vehicle.ModelName!.Trim(),

                Category =
                    NormalizeNullable(
                        finalJson.Vehicle.Category),

                BodyType =
                    NormalizeNullable(
                        finalJson.Vehicle.BodyType),

                ProductionBrandId = null,
                ProductionModelId = null
            };

            carsDbContext.ImportModels.Add(importModel);

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            result.ImportModelId =
                importModel.ImportModelId;

            // ========================================================
            // 3. POWERTRAINS
            // ========================================================

            var powertrainMap =
                new Dictionary<string, ImportPowertrain>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var dto in finalJson.Powertrains)
            {
                var powertrainRef =
                    NormalizeNullable(
                        dto.PowertrainRef);

                if (powertrainRef == null)
                {
                    throw new InvalidOperationException(
                        $"Powertrain IndexId {dto.IndexId} has no " +
                        "PowertrainRef after validation.");
                }

                var entity = new ImportPowertrain
                {
                    ImportModelId =
                        importModel.ImportModelId,

                    PowertrainRef =
                        powertrainRef,

                    PowertrainType =
                        NormalizePowertrainType(
                            dto.PowertrainType),

                    EngineRef =
                        NormalizeNullable(
                            dto.EngineRef),

                    BatteryCapacityKwh =
                        dto.BatteryCapacityKWh,

                    CombinedMaxPower =
                        dto.CombinedMaxPower,

                    CombinedMaxTorque =
                        dto.CombinedMaxTorque,

                    CombinedMaxPowerRpm =
                        dto.CombinedMaxPowerRPM,

                    CombinedMaxPowerRpmmin =
                        dto.CombinedMaxPowerRPMMin,

                    CombinedMaxPowerRpmmax =
                        dto.CombinedMaxPowerRPMMax,

                    CombinedMaxTorqueRpm =
                        dto.CombinedMaxTorqueRPM,

                    CombinedMaxTorqueRpmmin =
                        dto.CombinedMaxTorqueRPMMin,

                    CombinedMaxTorqueRpmmax =
                        dto.CombinedMaxTorqueRPMMax,

                    ProductionPowertrainId = null
                };

                carsDbContext.ImportPowertrains.Add(entity);

                powertrainMap.Add(
                    powertrainRef,
                    entity);

                result.PowertrainsInserted++;
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 4. ENGINES
            // ========================================================

            var engineMap =
                new Dictionary<string, ImportEngine>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var dto in finalJson.Engines)
            {
                var engineRef =
                    NormalizeNullable(
                        dto.EngineRef);

                if (engineRef == null)
                {
                    throw new InvalidOperationException(
                        $"Engine IndexId {dto.IndexId} has no EngineRef " +
                        "after validation.");
                }

                var powertrainRef =
                    GetPowertrainRefForEngine(
                        finalJson,
                        dto);

                if (!powertrainMap.TryGetValue(
                        powertrainRef,
                        out var powertrain))
                {
                    throw new InvalidOperationException(
                        $"Engine '{engineRef}' could not be mapped to " +
                        $"Powertrain '{powertrainRef}'.");
                }

                var engineName =
                    NormalizeNullable(dto.EngineName);

                if (engineName == null)
                {
                    throw new InvalidOperationException(
                        $"Engine '{engineRef}' has no EngineName " +
                        "after validation.");
                }

                var entity = new ImportEngine
                {
                    ImportPowertrainId =
                        powertrain.ImportPowertrainId,

                    EngineRef =
                        engineRef,

                    EngineName =
                        engineName,

                    NumberOfCylinders =
                        ToByte(dto.NumberOfCylinders),

                    NumberOfValves =
                        ToByte(dto.NumberOfValves),

                    Displacement =
                        dto.Displacement,

                    IsTurbocharged =
                        dto.IsTurbocharged,

                    EmissionStandard =
                        NormalizeNullable(
                            dto.EmissionStandard),

                    Aspiration =
                        NormalizeNullable(
                            dto.Aspiration),

                    EngineType =
                        NormalizeNullable(
                            dto.EngineType),

                    ProductionEngineId = null
                };

                carsDbContext.ImportEngines.Add(entity);

                engineMap.Add(
                    engineRef,
                    entity);

                result.EnginesInserted++;
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 5. ENGINE PERFORMANCE
            // ========================================================

            foreach (var engineDto in finalJson.Engines)
            {
                var engineRef =
                    NormalizeNullable(
                        engineDto.EngineRef);

                if (engineRef == null ||
                    !engineMap.TryGetValue(
                        engineRef,
                        out var engine))
                {
                    throw new InvalidOperationException(
                        $"Engine '{engineDto.EngineRef}' could not be " +
                        "mapped while staging EnginePerformance.");
                }

                foreach (var performanceDto
                    in engineDto.EnginePerformances)
                {
                    var modeName =
                        NormalizeNullable(
                            performanceDto.ModeName);

                    if (modeName == null)
                    {
                        throw new InvalidOperationException(
                            $"Engine '{engineRef}' contains an " +
                            "EnginePerformance without ModeName.");
                    }

                    var entity =
                        new ImportEnginePerformance
                        {
                            ImportEngineId =
                                engine.ImportEngineId,

                            ModeName =
                                modeName,

                            FuelTypeId =
                                performanceDto.FuelTypeId,

                            MaxPower =
                                performanceDto.MaxPower,

                            MaxTorque =
                                performanceDto.MaxTorque,

                            MaxPowerRpm =
                                performanceDto.MaxPowerRPM,

                            MaxPowerRpmmin =
                                performanceDto.MaxPowerRPMMin,

                            MaxPowerRpmmax =
                                performanceDto.MaxPowerRPMMax,

                            MaxTorqueRpm =
                                performanceDto.MaxTorqueRPM,

                            MaxTorqueRpmmin =
                                performanceDto.MaxTorqueRPMMin,

                            MaxTorqueRpmmax =
                                performanceDto.MaxTorqueRPMMax,

                            ProductionEnginePerformanceId = null
                        };

                    carsDbContext.ImportEnginePerformances.Add(
                        entity);

                    result.EnginePerformancesInserted++;
                }
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 6. MOTOR PERFORMANCE
            // ========================================================

            foreach (var powertrainDto
                in finalJson.Powertrains)
            {
                var powertrainRef =
                    NormalizeNullable(
                        powertrainDto.PowertrainRef);

                if (powertrainRef == null ||
                    !powertrainMap.TryGetValue(
                        powertrainRef,
                        out var powertrain))
                {
                    throw new InvalidOperationException(
                        $"Powertrain '{powertrainDto.PowertrainRef}' " +
                        "could not be mapped while staging " +
                        "MotorPerformance.");
                }

                foreach (var motorDto
                    in powertrainDto.MotorPerformances)
                {
                    var entity =
                        new ImportMotorPerformance
                        {
                            ImportPowertrainId =
                                powertrain.ImportPowertrainId,

                            MotorName =
                                NormalizeNullable(
                                    motorDto.MotorName),

                            MaxPower =
                                motorDto.MaxPower,

                            MaxTorque =
                                motorDto.MaxTorque,

                            MaxPowerRpm =
                                motorDto.MaxPowerRPM,

                            MaxPowerRpmmin =
                                motorDto.MaxPowerRPMMin,

                            MaxPowerRpmmax =
                                motorDto.MaxPowerRPMMax,

                            MaxTorqueRpm =
                                motorDto.MaxTorqueRPM,

                            MaxTorqueRpmmin =
                                motorDto.MaxTorqueRPMMin,

                            MaxTorqueRpmmax =
                                motorDto.MaxTorqueRPMMax,

                            ProductionMotorPerformanceId = null
                        };

                    carsDbContext.ImportMotorPerformances.Add(
                        entity);

                    result.MotorPerformancesInserted++;
                }
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 7. TRANSMISSIONS
            // ========================================================

            var transmissionMap =
                new Dictionary<string, ImportTransmission>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var dto in finalJson.Transmissions)
            {
                var transmissionRef =
                    NormalizeNullable(
                        dto.TransmissionRef);

                if (transmissionRef == null)
                {
                    throw new InvalidOperationException(
                        $"Transmission IndexId {dto.IndexId} has no " +
                        "TransmissionRef after validation.");
                }

                var entity = new ImportTransmission
                {
                    ImportModelId =
                        importModel.ImportModelId,

                    TransmissionRef =
                        transmissionRef,

                    TransmissionType =
                        NormalizeNullable(
                            dto.TransmissionType),

                    NumberOfGears =
                        dto.NumberOfGears,

                    HasManualOverride =
                        dto.HasManualOverride,

                    HasPaddleShifters =
                        dto.HasPaddleShifters,

                    ProductionTransmissionId = null
                };

                carsDbContext.ImportTransmissions.Add(entity);

                transmissionMap.Add(
                    transmissionRef,
                    entity);

                result.TransmissionsInserted++;
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 8. DRIVETRAINS
            // ========================================================

            var drivetrainMap =
                new Dictionary<string, ImportDrivetrain>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var dto in finalJson.Drivetrains)
            {
                var drivetrainRef =
                    NormalizeNullable(
                        dto.DrivetrainRef);

                if (drivetrainRef == null)
                {
                    throw new InvalidOperationException(
                        $"Drivetrain IndexId {dto.IndexId} has no " +
                        "DrivetrainRef after validation.");
                }

                var entity = new ImportDrivetrain
                {
                    ImportModelId =
                        importModel.ImportModelId,

                    DrivetrainRef =
                        drivetrainRef,

                    DrivetrainType =
                        NormalizeNullable(
                            dto.DrivetrainType),

                    DifferentialType =
                        NormalizeNullable(
                            dto.DifferentialType),

                    ProductionDrivetrainId = null
                };

                carsDbContext.ImportDrivetrains.Add(entity);

                drivetrainMap.Add(
                    drivetrainRef,
                    entity);

                result.DrivetrainsInserted++;
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 9. LOGICAL VARIANT PARENTS
            // ========================================================

            var logicalParentMap =
                BuildLogicalParentMap(
                    finalJson,
                    validation.Errors);

            if (validation.Errors.Count > 0)
            {
                throw new ImportBatchValidationException(
                    importBatchId,
                    validation.Errors);
            }

            var parentMap =
                new Dictionary<string, ImportVariantParent>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var parentName in logicalParentMap.Values
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .OrderBy(x => x))
            {
                var entity =
                    new ImportVariantParent
                    {
                        ImportModelId =
                            importModel.ImportModelId,

                        ParentVariantName =
                            parentName
                    };

                carsDbContext.ImportVariantParents.Add(entity);

                parentMap.Add(
                    parentName,
                    entity);

                result.VariantParentsInserted++;
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 10. VARIANTS
            // ========================================================

            var variantMap =
                new Dictionary<int, ImportVariant>();

            foreach (var dto in finalJson.Variants)
            {
                if (!logicalParentMap.TryGetValue(
                        dto.IndexId,
                        out var logicalParentName))
                {
                    throw new InvalidOperationException(
                        $"Variant '{dto.VariantName}' has no logical " +
                        "parent mapping.");
                }

                if (!parentMap.TryGetValue(
                        logicalParentName,
                        out var parent))
                {
                    throw new InvalidOperationException(
                        $"Logical parent '{logicalParentName}' for " +
                        $"variant '{dto.VariantName}' was not staged.");
                }

                ImportPowertrain? powertrain = null;
                ImportTransmission? transmission = null;
                ImportDrivetrain? drivetrain = null;

                var powertrainRef =
                    NormalizeNullable(
                        dto.PowertrainRef);

                var transmissionRef =
                    NormalizeNullable(
                        dto.TransmissionRef);

                var drivetrainRef =
                    NormalizeNullable(
                        dto.DrivetrainRef);

                if (powertrainRef != null &&
                    !powertrainMap.TryGetValue(
                        powertrainRef,
                        out powertrain))
                {
                    throw new InvalidOperationException(
                        $"Variant '{dto.VariantName}' references " +
                        $"PowertrainRef '{powertrainRef}', but it was " +
                        "not staged.");
                }

                if (transmissionRef != null &&
                    !transmissionMap.TryGetValue(
                        transmissionRef,
                        out transmission))
                {
                    throw new InvalidOperationException(
                        $"Variant '{dto.VariantName}' references " +
                        $"TransmissionRef '{transmissionRef}', but it " +
                        "was not staged.");
                }

                // Drivetrain is intentionally optional.
                if (drivetrainRef != null &&
                    !drivetrainMap.TryGetValue(
                        drivetrainRef,
                        out drivetrain))
                {
                    throw new InvalidOperationException(
                        $"Variant '{dto.VariantName}' references " +
                        $"DrivetrainRef '{drivetrainRef}', but it was " +
                        "not staged.");
                }

                var variantName =
                    NormalizeNullable(dto.VariantName);

                if (variantName == null)
                {
                    throw new InvalidOperationException(
                        $"Variant IndexId {dto.IndexId} has no " +
                        "VariantName after validation.");
                }

                if (string.Equals(dto.VariantType, "parent", StringComparison.OrdinalIgnoreCase))
                    continue;

                var entity =
                    new ImportVariant
                    {
                        ImportVariantParentId =
                            parent.ImportVariantParentId,

                        VariantName =
                            variantName,

                        VariantType =
                            NormalizeVariantType(
                                dto.VariantType),

                        BaseVariantName =
                            NormalizeNullable(
                                dto.BaseVariantName),

                        PowertrainRef =
                            powertrainRef,

                        TransmissionRef =
                            transmissionRef,

                        DrivetrainRef =
                            drivetrainRef,

                        ExShowroomPrice =
                            dto.ExShowroomPrice,

                        KerbWeight =
                            dto.KerbWeight,

                        SeatingCapacity =
                            dto.SeatingCapacity,

                        ProductionVariantId = null,

                        ImportPowertrainId =
                            powertrain?.ImportPowertrainId,

                        ImportTransmissionId =
                            transmission?.ImportTransmissionId,

                        ImportDrivetrainId =
                            drivetrain?.ImportDrivetrainId
                    };

                carsDbContext.ImportVariants.Add(entity);

                variantMap.Add(
                    dto.IndexId,
                    entity);

                result.VariantsInserted++;
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 11. FEATURES
            //
            // ImportFeature is already the copied master catalog.
            //
            // ImportFeatureVariant contains the actual observation.
            //
            // ImportFeatureValueOption now belongs to the
            // ImportFeatureVariant.
            // ========================================================

            var featureVariantRows =
                new List<(FeatureDto Dto, ImportFeatureVariant Entity)>();

            foreach (var dto in finalJson.Features)
            {
                var importFeature =
                    await carsDbContext.ImportFeatures
                        .FirstOrDefaultAsync(
                            x =>
                                x.ImportFeatureId ==
                                dto.FeatureId,
                            cancellationToken);

                if (importFeature == null)
                {
                    throw new InvalidOperationException(
                        $"FeatureId {dto.FeatureId} does not exist " +
                        "in ImportFeature.");
                }

                foreach (var variantName
                    in dto.AppliesToVariants)
                {
                    var variant =
                        GetVariantByName(
                            finalJson,
                            variantMap,
                            variantName);

                    var entity =
                        new ImportFeatureVariant
                        {
                            ImportFeatureId =
                                importFeature.ImportFeatureId,

                            ImportVariantId =
                                variant.ImportVariantId,

                            Available =
                                dto.Available,

                            Value =
                                dto.Value,

                            SourceColumn =
                                dto.SourceColumn,

                            PageNumber =
                                dto.PageNumber,

                            Evidence =
                                dto.Evidence,

                            Confidence =
                                dto.Confidence
                        };

                    carsDbContext.ImportFeatureVariants.Add(
                        entity);

                    featureVariantRows.Add(
                        (dto, entity));

                    result.FeatureVariantsInserted++;
                }
            }

            // Save once so every ImportFeatureVariant identity is known.
            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // --------------------------------------------------------
            // Feature Value Options
            //
            // Example:
            //
            // AI:
            //     ValueOptionIds = [1,3,4]
            //
            // Database:
            //
            // ImportFeatureVariantId | ValueOptionId
            // -----------------------+--------------
            // 101                    | 1
            // 101                    | 3
            // 101                    | 4
            // --------------------------------------------------------

            foreach (var item in featureVariantRows)
            {
                var optionIds =
                    item.Dto.ValueOptionIds
                        .Distinct()
                        .ToList();

                foreach (var valueOptionId in optionIds)
                {
                    carsDbContext.ImportFeatureValueOptions.Add(
                        new ImportFeatureValueOption
                        {
                            ImportFeatureVariantId =
                                item.Entity.ImportFeatureVariantId,

                            ValueOptionId =
                                valueOptionId
                        });
                }
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 12. SPECIFICATIONS
            // ========================================================

            foreach (var dto in finalJson.Specifications)
            {
                var importSpecification =
                    await carsDbContext.ImportSpecifications
                        .FirstOrDefaultAsync(
                            x =>
                                x.ImportSpecificationId ==
                                dto.SpecificationId,
                            cancellationToken);

                if (importSpecification == null)
                {
                    throw new InvalidOperationException(
                        $"SpecificationId {dto.SpecificationId} does not " +
                        "exist in ImportSpecification.");
                }

                foreach (var variantName
                    in dto.AppliesToVariants)
                {
                    var variant =
                        GetVariantByName(
                            finalJson,
                            variantMap,
                            variantName);

                    var entity =
                        new ImportSpecificationVariant
                        {
                            ImportSpecificationId =
                                importSpecification
                                    .ImportSpecificationId,

                            ImportVariantId =
                                variant.ImportVariantId,

                            NumericValue =
                                dto.NumericValue,

                            TextValue =
                                dto.TextValue,

                            BooleanValue =
                                dto.BooleanValue,

                            Unit =
                                dto.Unit,

                            SourceColumn =
                                dto.SourceColumn,

                            PageNumber =
                                dto.PageNumber,

                            Evidence =
                                dto.Evidence,

                            Confidence =
                                dto.Confidence
                        };

                    carsDbContext.ImportSpecificationVariants.Add(
                        entity);

                    result.SpecificationVariantsInserted++;
                }
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 13. FUEL EFFICIENCY
            // ========================================================

            foreach (var dto in finalJson.FuelEfficiencies)
            {
                if (!dto.FuelEfficiency.HasValue)
                {
                    throw new InvalidOperationException(
                        $"FuelEfficiency IndexId {dto.IndexId} has no " +
                        "FuelEfficiency after validation.");
                }

                var unit =
                    NormalizeNullable(
                        dto.FuelEfficiencyUnit);

                if (unit == null)
                {
                    throw new InvalidOperationException(
                        $"FuelEfficiency IndexId {dto.IndexId} has no " +
                        "FuelEfficiencyUnit after validation.");
                }

                var fuelEfficiency =
                    new ImportFuelEfficiency
                    {
                        FuelEfficiency =
                            dto.FuelEfficiency.Value,

                        FuelEfficiencyUnit =
                            unit,

                        SourceColumn =
                            dto.SourceColumn,

                        PageNumber =
                            dto.PageNumber,

                        Evidence =
                            dto.Evidence,

                        Confidence =
                            dto.Confidence,

                        ProductionFuelEfficiencyId =
                            null
                    };

                carsDbContext.ImportFuelEfficiencies.Add(
                    fuelEfficiency);

                await carsDbContext.SaveChangesAsync(
                    cancellationToken);

                result.FuelEfficienciesInserted++;

                var insertedVariantIds =
                    new HashSet<int>();

                foreach (var variantName
                    in dto.AppliesToVariants)
                {
                    var variant =
                        GetVariantByName(
                            finalJson,
                            variantMap,
                            variantName);

                    if (!insertedVariantIds.Add(
                            variant.ImportVariantId))
                    {
                        throw new InvalidOperationException(
                            $"FuelEfficiency IndexId {dto.IndexId} " +
                            $"contains duplicate variant '{variantName}'.");
                    }

                    carsDbContext.ImportFuelEfficiencyVariants.Add(
                        new ImportFuelEfficiencyVariant
                        {
                            ImportFuelEfficiencyId =
                                fuelEfficiency
                                    .ImportFuelEfficiencyId,

                            ImportVariantId =
                                variant.ImportVariantId
                        });

                    result.FuelEfficiencyVariantsInserted++;
                }
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 14. WARNINGS
            //
            // Warnings are intentionally never validation failures.
            // ========================================================

            foreach (var warningText in finalJson.Warnings
                         .Where(x =>
                             !string.IsNullOrWhiteSpace(x)))
            {
                carsDbContext.ImportWarnings.Add(
                    new ImportWarning
                    {
                        ImportBatchId =
                            batch.ImportBatchId,

                        ImportRecordId =
                            modelRecord.ImportRecordId,

                        WarningText =
                            warningText.Trim(),

                        Status =
                            "Pending"
                    });

                result.WarningsInserted++;
            }

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            // ========================================================
            // 15. FINAL STATUS
            // ========================================================

            batch.Status = "Staged";

            await carsDbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            result.Status = batch.Status;

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    // ============================================================
    // COMPLETE FINAL JSON VALIDATION
    // ============================================================

    private async Task<StageValidationResult> ValidateFinalJsonAsync(
        ImportBatch batch,
        BrochureExtractionDto json,
        CancellationToken cancellationToken)
    {
        var result = new StageValidationResult();

        // ========================================================
        // VEHICLE
        // ========================================================

        if (json.Vehicle == null)
        {
            result.Errors.Add(
                "Vehicle object is required.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(
                    json.Vehicle.Brand))
            {
                result.Errors.Add(
                    "Vehicle.Brand is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    json.Vehicle.ModelName))
            {
                result.Errors.Add(
                    "Vehicle.ModelName is required.");
            }
        }

        // ========================================================
        // INDEX IDs
        // ========================================================

        ValidateIndexIds(
            json.Powertrains.Select(x => x.IndexId),
            "Powertrain",
            result);

        ValidateIndexIds(
            json.Engines.Select(x => x.IndexId),
            "Engine",
            result);

        ValidateIndexIds(
            json.Transmissions.Select(x => x.IndexId),
            "Transmission",
            result);

        ValidateIndexIds(
            json.Drivetrains.Select(x => x.IndexId),
            "Drivetrain",
            result);

        ValidateIndexIds(
            json.Variants.Select(x => x.IndexId),
            "Variant",
            result);

        ValidateIndexIds(
            json.Features.Select(x => x.IndexId),
            "Feature",
            result);

        ValidateIndexIds(
            json.Specifications.Select(x => x.IndexId),
            "Specification",
            result);

        ValidateIndexIds(
            json.FuelEfficiencies.Select(x => x.IndexId),
            "FuelEfficiency",
            result);

        // ========================================================
        // POWERTRAINS
        // ========================================================

        var powertrainRefs =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var powertrain in json.Powertrains)
        {
            var powertrainRef =
                NormalizeNullable(
                    powertrain.PowertrainRef);

            if (powertrainRef == null)
            {
                result.Errors.Add(
                    $"Powertrain IndexId {powertrain.IndexId}: " +
                    "PowertrainRef is required.");

                continue;
            }

            if (!powertrainRefs.Add(
                    powertrainRef))
            {
                result.Errors.Add(
                    $"Duplicate PowertrainRef '{powertrainRef}'.");
            }

            var normalizedType =
                NormalizePowertrainTypeOrNull(
                    powertrain.PowertrainType);

            if (normalizedType == null)
            {
                result.Errors.Add(
                    $"Powertrain '{powertrainRef}': invalid " +
                    $"PowertrainType '{powertrain.PowertrainType}'. " +
                    "Allowed values are ICE, EV and Hybrid.");
            }

            if (!string.IsNullOrWhiteSpace(
                    powertrain.EngineRef))
            {
                var engineRef =
                    powertrain.EngineRef.Trim();

                var matchingEngines =
                    json.Engines
                        .Where(x =>
                            string.Equals(
                                x.EngineRef?.Trim(),
                                engineRef,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                if (matchingEngines.Count == 0)
                {
                    result.Errors.Add(
                        $"Powertrain '{powertrainRef}' references " +
                        $"EngineRef '{engineRef}', but that engine " +
                        "does not exist.");
                }
                else if (matchingEngines.Count > 1)
                {
                    result.Errors.Add(
                        $"Powertrain '{powertrainRef}' references " +
                        $"EngineRef '{engineRef}', but duplicate " +
                        "EngineRef records exist.");
                }
            }
        }

        // ========================================================
        // ENGINES
        // ========================================================

        var engineRefs =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var engine in json.Engines)
        {
            var engineRef =
                NormalizeNullable(
                    engine.EngineRef);

            if (engineRef == null)
            {
                result.Errors.Add(
                    $"Engine IndexId {engine.IndexId}: " +
                    "EngineRef is required.");

                continue;
            }

            if (!engineRefs.Add(
                    engineRef))
            {
                result.Errors.Add(
                    $"Duplicate EngineRef '{engineRef}'.");
            }

            if (string.IsNullOrWhiteSpace(
                    engine.EngineName))
            {
                result.Errors.Add(
                    $"Engine '{engineRef}': EngineName is required.");
            }
        }

        // --------------------------------------------------------
        // Load all referenced FuelTypes once.
        // Avoid one SQL query per EnginePerformance.
        // --------------------------------------------------------

        var requestedFuelTypeIds =
            json.Engines
                .SelectMany(x => x.EnginePerformances)
                .Where(x => x.FuelTypeId.HasValue)
                .Select(x => x.FuelTypeId!.Value)
                .Distinct()
                .ToList();

        if (requestedFuelTypeIds.Count > 0)
        {
            var existingFuelTypeIds =
                await carsDbContext.FuelTypes
                    .Where(x =>
                        requestedFuelTypeIds.Contains(
                            x.FuelTypeId))
                    .Select(x => x.FuelTypeId)
                    .ToListAsync(
                        cancellationToken);

            var existingFuelTypeSet =
                existingFuelTypeIds.ToHashSet();

            foreach (var engine in json.Engines)
            {
                var engineRef =
                    NormalizeNullable(
                        engine.EngineRef);

                if (engineRef == null)
                    continue;

                foreach (var performance
                    in engine.EnginePerformances)
                {
                    if (string.IsNullOrWhiteSpace(
                            performance.ModeName))
                    {
                        result.Errors.Add(
                            $"Engine '{engineRef}' has an " +
                            "EnginePerformance with empty ModeName.");
                    }

                    if (performance.FuelTypeId.HasValue &&
                        !existingFuelTypeSet.Contains(
                            performance.FuelTypeId.Value))
                    {
                        result.Errors.Add(
                            $"Engine '{engineRef}' performance " +
                            $"references FuelTypeId " +
                            $"{performance.FuelTypeId.Value}, which " +
                            "does not exist.");
                    }
                }
            }
        }
        else
        {
            foreach (var engine in json.Engines)
            {
                var engineRef =
                    NormalizeNullable(
                        engine.EngineRef);

                if (engineRef == null)
                    continue;

                foreach (var performance
                    in engine.EnginePerformances)
                {
                    if (string.IsNullOrWhiteSpace(
                            performance.ModeName))
                    {
                        result.Errors.Add(
                            $"Engine '{engineRef}' has an " +
                            "EnginePerformance with empty ModeName.");
                    }
                }
            }
        }

        // Every engine must belong to exactly one powertrain.
        foreach (var engine in json.Engines)
        {
            var engineRef =
                NormalizeNullable(
                    engine.EngineRef);

            if (engineRef == null)
                continue;

            var referencingPowertrains =
                json.Powertrains
                    .Where(x =>
                        string.Equals(
                            x.EngineRef?.Trim(),
                            engineRef,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (referencingPowertrains.Count == 0)
            {
                result.Errors.Add(
                    $"Engine '{engineRef}' is not referenced by " +
                    "any Powertrain.EngineRef.");
            }
            else if (referencingPowertrains.Count > 1)
            {
                result.Errors.Add(
                    $"Engine '{engineRef}' is referenced by multiple " +
                    "powertrains. An ImportEngine belongs to exactly " +
                    "one ImportPowertrain.");
            }
        }

        foreach (var powertrain in json.Powertrains)
        {
            var type =
                NormalizePowertrainTypeOrNull(
                    powertrain.PowertrainType);

            if (type == "ICE" || type == "Hybrid")
            {
                if (string.IsNullOrWhiteSpace(
                        powertrain.EngineRef))
                {
                    result.Errors.Add(
                        $"Powertrain '{powertrain.PowertrainRef}' is " +
                        $"{type} but has no EngineRef.");
                }
            }
        }

        // ========================================================
        // TRANSMISSIONS
        // ========================================================

        var transmissionRefs =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var transmission in json.Transmissions)
        {
            var transmissionRef =
                NormalizeNullable(
                    transmission.TransmissionRef);

            if (transmissionRef == null)
            {
                result.Errors.Add(
                    $"Transmission IndexId {transmission.IndexId}: " +
                    "TransmissionRef is required.");

                continue;
            }

            if (!transmissionRefs.Add(
                    transmissionRef))
            {
                result.Errors.Add(
                    $"Duplicate TransmissionRef " +
                    $"'{transmissionRef}'.");
            }
        }

        // ========================================================
        // DRIVETRAINS
        // ========================================================

        var drivetrainRefs =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var drivetrain in json.Drivetrains)
        {
            var drivetrainRef =
                NormalizeNullable(
                    drivetrain.DrivetrainRef);

            if (drivetrainRef == null)
            {
                result.Errors.Add(
                    $"Drivetrain IndexId {drivetrain.IndexId}: " +
                    "DrivetrainRef is required when a drivetrain " +
                    "record exists.");

                continue;
            }

            if (!drivetrainRefs.Add(
                    drivetrainRef))
            {
                result.Errors.Add(
                    $"Duplicate DrivetrainRef " +
                    $"'{drivetrainRef}'.");
            }
        }

        // ========================================================
        // VARIANTS
        // ========================================================

        ValidateVariantHierarchy(
            json,
            result);

        var variantNames =
            new HashSet<string>(
                json.Variants
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.VariantName))
                    .Select(x =>
                        x.VariantName!.Trim()),
                StringComparer.OrdinalIgnoreCase);

        foreach (var variant in json.Variants)
        {
            if (string.IsNullOrWhiteSpace(
                    variant.VariantName))
            {
                result.Errors.Add(
                    $"Variant IndexId {variant.IndexId}: " +
                    "VariantName is required.");

                continue;
            }

            if (!string.IsNullOrWhiteSpace(
                    variant.PowertrainRef)
                &&
                !powertrainRefs.Contains(
                    variant.PowertrainRef.Trim()))
            {
                result.Errors.Add(
                    $"Variant '{variant.VariantName}' references " +
                    $"unknown PowertrainRef " +
                    $"'{variant.PowertrainRef}'.");
            }

            if (!string.IsNullOrWhiteSpace(
                    variant.TransmissionRef)
                &&
                !transmissionRefs.Contains(
                    variant.TransmissionRef.Trim()))
            {
                result.Errors.Add(
                    $"Variant '{variant.VariantName}' references " +
                    $"unknown TransmissionRef " +
                    $"'{variant.TransmissionRef}'.");
            }

            // Drivetrain is optional.
            if (!string.IsNullOrWhiteSpace(
                    variant.DrivetrainRef)
                &&
                !drivetrainRefs.Contains(
                    variant.DrivetrainRef.Trim()))
            {
                result.Errors.Add(
                    $"Variant '{variant.VariantName}' references " +
                    $"unknown DrivetrainRef " +
                    $"'{variant.DrivetrainRef}'.");
            }

            if (!string.IsNullOrWhiteSpace(
                    variant.BaseVariantName)
                &&
                !variantNames.Contains(
                    variant.BaseVariantName.Trim()))
            {
                result.Errors.Add(
                    $"Variant '{variant.VariantName}' references " +
                    $"unknown BaseVariantName " +
                    $"'{variant.BaseVariantName}'.");
            }
        }

        // ========================================================
        // FEATURES
        // ========================================================

        await ValidateFeaturesAsync(
            json,
            result,
            cancellationToken);

        // ========================================================
        // SPECIFICATIONS
        // ========================================================

        await ValidateSpecificationsAsync(
            json,
            result,
            cancellationToken);

        // ========================================================
        // FUEL EFFICIENCY
        // ========================================================

        ValidateFuelEfficiencies(
            json,
            variantNames,
            result);

        return result;
    }

    // ============================================================
    // FEATURE VALIDATION
    // ============================================================

    private async Task ValidateFeaturesAsync(
        BrochureExtractionDto json,
        StageValidationResult result,
        CancellationToken cancellationToken)
    {
        var featureIds =
            json.Features
                .Select(x => x.FeatureId)
                .Where(x => x > 0)
                .Distinct()
                .ToList();

        var features =
            featureIds.Count == 0
                ? new List<ImportFeature>()
                : await carsDbContext.ImportFeatures
                    .Include(x => x.Feature)
                    .Where(x =>
                        featureIds.Contains(
                            x.ImportFeatureId))
                    .ToListAsync(
                        cancellationToken);

        var featureMap =
            features.ToDictionary(
                x => x.ImportFeatureId);

        var variantMap =
            json.Variants
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.VariantName))
                .GroupBy(
                    x => x.VariantName!.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    x => x.Key,
                    x => x.First(),
                    StringComparer.OrdinalIgnoreCase);

        var valueOptionIds =
            json.Features
                .SelectMany(x =>
                    x.ValueOptionIds ?? [])
                .Where(x => x > 0)
                .Distinct()
                .ToList();

        var valueOptions =
            valueOptionIds.Count == 0
                ? new List<FeatureValueOption>()
                : await carsDbContext.FeatureValueOptions
                    .Where(x =>
                        valueOptionIds.Contains(
                            x.FeatureValueOptionId))
                    .ToListAsync(
                        cancellationToken);

        var valueOptionMap =
            valueOptions.ToDictionary(
                x => x.FeatureValueOptionId);

        foreach (var dto in json.Features)
        {
            if (dto.FeatureId <= 0)
            {
                result.Errors.Add(
                    $"Feature IndexId {dto.IndexId}: " +
                    "FeatureId must be greater than zero.");

                continue;
            }

            if (string.IsNullOrWhiteSpace(
                    dto.FeatureCode))
            {
                result.Errors.Add(
                    $"FeatureId {dto.FeatureId}: " +
                    "FeatureCode is required.");
            }

            if (!featureMap.TryGetValue(
                    dto.FeatureId,
                    out var importFeature))
            {
                result.Errors.Add(
                    $"FeatureId {dto.FeatureId} with code " +
                    $"'{dto.FeatureCode}' does not exist " +
                    "in ImportFeature.");

                continue;
            }

            if (!string.Equals(
                    importFeature.FeatureCode?.Trim(),
                    dto.FeatureCode?.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add(
                    $"FeatureId {dto.FeatureId} has FeatureCode " +
                    $"'{dto.FeatureCode}' in FinalJson, but the " +
                    $"master ImportFeature contains " +
                    $"'{importFeature.FeatureCode}'.");
            }

            if (importFeature.FeatureId != dto.FeatureId)
            {
                result.Errors.Add(
                    $"ImportFeatureId {dto.FeatureId} has mismatched " +
                    $"FeatureId {importFeature.FeatureId}.");
            }

            // ----------------------------------------------------
            // Applicability
            // ----------------------------------------------------

            if (dto.AppliesToVariants == null ||
                dto.AppliesToVariants.Count == 0)
            {
                result.Errors.Add(
                    $"Feature '{dto.FeatureCode}' has no " +
                    "AppliesToVariants.");
            }
            else
            {
                var seenVariants =
                    new HashSet<string>(
                        StringComparer.OrdinalIgnoreCase);

                foreach (var variantName
                    in dto.AppliesToVariants)
                {
                    if (string.IsNullOrWhiteSpace(
                            variantName))
                    {
                        result.Errors.Add(
                            $"Feature '{dto.FeatureCode}' contains " +
                            "an empty AppliesToVariants value.");

                        continue;
                    }

                    var trimmedName =
                        variantName.Trim();

                    if (!seenVariants.Add(
                            trimmedName))
                    {
                        result.Errors.Add(
                            $"Feature '{dto.FeatureCode}' contains " +
                            $"duplicate applicability variant " +
                            $"'{trimmedName}'.");
                    }

                    if (!variantMap.TryGetValue(
                            trimmedName,
                            out var variant))
                    {
                        result.Errors.Add(
                            $"Feature '{dto.FeatureCode}' references " +
                            $"unknown variant '{trimmedName}'.");
                    }
                    else if (IsParentVariant(variant))
                    {
                        result.Errors.Add(
                            $"Feature '{dto.FeatureCode}' applies " +
                            $"to parent variant '{trimmedName}'. " +
                            "Feature applicability should resolve to " +
                            "concrete subVariants.");
                    }
                }
            }

            // ----------------------------------------------------
            // ValueOptionIds
            // ----------------------------------------------------

            var optionIds =
                dto.ValueOptionIds ?? [];

            if (optionIds.Count !=
                optionIds.Distinct().Count())
            {
                result.Errors.Add(
                    $"Feature '{dto.FeatureCode}' contains duplicate " +
                    "ValueOptionIds.");
            }

            foreach (var valueOptionId in optionIds)
            {
                if (!valueOptionMap.TryGetValue(
                        valueOptionId,
                        out var option))
                {
                    result.Errors.Add(
                        $"Feature '{dto.FeatureCode}' references " +
                        $"ValueOptionId {valueOptionId}, which does " +
                        "not exist.");

                    continue;
                }

                if (option.FeatureId != dto.FeatureId)
                {
                    result.Errors.Add(
                        $"Feature '{dto.FeatureCode}' (FeatureId " +
                        $"{dto.FeatureId}) references ValueOptionId " +
                        $"{valueOptionId}, but that option belongs to " +
                        $"FeatureId {option.FeatureId}.");
                }
            }

            // ----------------------------------------------------
            // IMPORTANT:
            //
            // ValueType and IsMultiValue belong to the production
            // Feature master, not ImportFeature.
            // ----------------------------------------------------

            var productionFeature =
                importFeature.Feature;

            if (productionFeature == null)
            {
                result.Errors.Add(
                    $"Feature '{dto.FeatureCode}' (FeatureId " +
                    $"{dto.FeatureId}) has no linked production Feature.");

                continue;
            }

            var valueType =
                productionFeature.ValueType?.Trim();

            var isEnum =
                string.Equals(
                    valueType,
                    "enum",
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    valueType,
                    "select",
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    valueType,
                    "multiselect",
                    StringComparison.OrdinalIgnoreCase);

            var isBoolean =
                string.Equals(
                    valueType,
                    "boolean",
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    valueType,
                    "bool",
                    StringComparison.OrdinalIgnoreCase);

            if (isBoolean &&
                optionIds.Count > 0)
            {
                result.Errors.Add(
                    $"Boolean feature '{dto.FeatureCode}' " +
                    "must not contain ValueOptionIds.");
            }

            if (isEnum &&
                dto.Available == true &&
                optionIds.Count == 0)
            {
                result.Errors.Add(
                    $"Enum feature '{dto.FeatureCode}' is marked " +
                    "available but contains no ValueOptionIds.");
            }

            if (isEnum &&
                !productionFeature.IsMultiValue &&
                optionIds.Count > 1)
            {
                result.Errors.Add(
                    $"Single-value enum feature '{dto.FeatureCode}' " +
                    $"contains {optionIds.Count} ValueOptionIds. " +
                    "Only one option is allowed.");
            }
        }
    }

    // ============================================================
    // SPECIFICATION VALIDATION
    // ============================================================

    private async Task ValidateSpecificationsAsync(
        BrochureExtractionDto json,
        StageValidationResult result,
        CancellationToken cancellationToken)
    {
        var specificationIds =
            json.Specifications
                .Select(x => x.SpecificationId)
                .Where(x => x > 0)
                .Distinct()
                .ToList();

        var specifications =
            specificationIds.Count == 0
                ? new List<ImportSpecification>()
                : await carsDbContext.ImportSpecifications
                    .Where(x =>
                        specificationIds.Contains(
                            x.ImportSpecificationId))
                    .ToListAsync(
                        cancellationToken);

        var specificationMap =
            specifications.ToDictionary(
                x => x.ImportSpecificationId);

        var variants =
            json.Variants
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.VariantName))
                .GroupBy(
                    x => x.VariantName!.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    x => x.Key,
                    x => x.First(),
                    StringComparer.OrdinalIgnoreCase);

        foreach (var dto in json.Specifications)
        {
            if (dto.SpecificationId <= 0)
            {
                result.Errors.Add(
                    $"Specification IndexId {dto.IndexId}: " +
                    "SpecificationId must be greater than zero.");

                continue;
            }

            if (string.IsNullOrWhiteSpace(
                    dto.SpecificationCode))
            {
                result.Errors.Add(
                    $"SpecificationId {dto.SpecificationId}: " +
                    "SpecificationCode is required.");
            }

            if (!specificationMap.TryGetValue(
                    dto.SpecificationId,
                    out var importSpecification))
            {
                result.Errors.Add(
                    $"SpecificationId {dto.SpecificationId} with code " +
                    $"'{dto.SpecificationCode}' does not exist " +
                    "in ImportSpecification.");

                continue;
            }

            if (!string.Equals(
                    importSpecification.SpecificationCode?.Trim(),
                    dto.SpecificationCode?.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add(
                    $"SpecificationId {dto.SpecificationId} has code " +
                    $"'{dto.SpecificationCode}' in FinalJson, but the " +
                    $"master ImportSpecification contains " +
                    $"'{importSpecification.SpecificationCode}'.");
            }

            if (importSpecification.SpecificationId !=
                dto.SpecificationId)
            {
                result.Errors.Add(
                    $"ImportSpecificationId " +
                    $"{dto.SpecificationId} has mismatched " +
                    $"SpecificationId " +
                    $"{importSpecification.SpecificationId}.");
            }

            if (dto.AppliesToVariants == null ||
                dto.AppliesToVariants.Count == 0)
            {
                result.Errors.Add(
                    $"Specification '{dto.SpecificationCode}' has no " +
                    "AppliesToVariants.");
            }
            else
            {
                var seenVariants =
                    new HashSet<string>(
                        StringComparer.OrdinalIgnoreCase);

                foreach (var variantName
                    in dto.AppliesToVariants)
                {
                    if (string.IsNullOrWhiteSpace(
                            variantName))
                    {
                        result.Errors.Add(
                            $"Specification '{dto.SpecificationCode}' " +
                            "contains an empty AppliesToVariants value.");

                        continue;
                    }

                    var trimmedName =
                        variantName.Trim();

                    if (!seenVariants.Add(
                            trimmedName))
                    {
                        result.Errors.Add(
                            $"Specification '{dto.SpecificationCode}' " +
                            $"contains duplicate applicability variant " +
                            $"'{trimmedName}'.");
                    }

                    if (!variants.TryGetValue(
                            trimmedName,
                            out var variant))
                    {
                        result.Errors.Add(
                            $"Specification '{dto.SpecificationCode}' " +
                            $"references unknown variant " +
                            $"'{trimmedName}'.");
                    }
                    else if (IsParentVariant(variant))
                    {
                        result.Errors.Add(
                            $"Specification '{dto.SpecificationCode}' " +
                            $"applies to parent variant '{trimmedName}'. " +
                            "Specification applicability should resolve " +
                            "to concrete subVariants.");
                    }
                }
            }

            //var valueCount = 0;

            //if (dto.NumericValue.HasValue)
            //    valueCount++;

            //if (dto.TextValue != null)
            //    valueCount++;

            //if (dto.BooleanValue.HasValue)
            //    valueCount++;

            //if (valueCount > 1)
            //{
            //    result.Errors.Add(
            //        $"Specification '{dto.SpecificationCode}' has " +
            //        "multiple value representations populated. " +
            //        "NumericValue, TextValue and BooleanValue are " +
            //        "mutually exclusive.");
            //}
        }
    }

    // ============================================================
    // FUEL EFFICIENCY VALIDATION
    // ============================================================

    private static void ValidateFuelEfficiencies(
        BrochureExtractionDto json,
        HashSet<string> variantNames,
        StageValidationResult result)
    {
        foreach (var dto in json.FuelEfficiencies)
        {
            if (!dto.FuelEfficiency.HasValue)
            {
                result.Errors.Add(
                    $"FuelEfficiency IndexId {dto.IndexId}: " +
                    "FuelEfficiency cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.FuelEfficiencyUnit))
            {
                result.Errors.Add(
                    $"FuelEfficiency IndexId {dto.IndexId}: " +
                    "FuelEfficiencyUnit is required.");
            }

            if (dto.AppliesToVariants == null ||
                dto.AppliesToVariants.Count == 0)
            {
                result.Errors.Add(
                    $"FuelEfficiency IndexId {dto.IndexId}: " +
                    "AppliesToVariants is required.");

                continue;
            }

            var seen =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var variantName
                in dto.AppliesToVariants)
            {
                if (string.IsNullOrWhiteSpace(
                        variantName))
                {
                    result.Errors.Add(
                        $"FuelEfficiency IndexId {dto.IndexId} " +
                        "contains an empty variant name.");

                    continue;
                }

                var trimmedName =
                    variantName.Trim();

                if (!seen.Add(
                        trimmedName))
                {
                    result.Errors.Add(
                        $"FuelEfficiency IndexId {dto.IndexId} " +
                        $"contains duplicate variant " +
                        $"'{trimmedName}'.");
                }

                if (!variantNames.Contains(
                        trimmedName))
                {
                    result.Errors.Add(
                        $"FuelEfficiency IndexId {dto.IndexId} " +
                        $"references unknown variant " +
                        $"'{trimmedName}'.");
                }
            }
        }
    }

    // ============================================================
    // VARIANT HIERARCHY VALIDATION
    // ============================================================

    private static void ValidateVariantHierarchy(
        BrochureExtractionDto json,
        StageValidationResult result)
    {
        if (json.Variants == null ||
            json.Variants.Count == 0)
        {
            result.Errors.Add(
                "FinalJson must contain at least one variant.");

            return;
        }

        var variantsByName =
            json.Variants
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.VariantName))
                .GroupBy(
                    x => x.VariantName!.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList(),
                    StringComparer.OrdinalIgnoreCase);

        foreach (var group in variantsByName)
        {
            if (group.Value.Count > 1)
            {
                result.Errors.Add(
                    $"Duplicate VariantName '{group.Key}'. " +
                    "Every variant must have a unique name.");
            }
        }

        foreach (var variant in json.Variants)
        {
            var name =
                NormalizeNullable(
                    variant.VariantName);

            if (name == null)
            {
                result.Errors.Add(
                    $"Variant IndexId {variant.IndexId}: " +
                    "VariantName is required.");

                continue;
            }

            var type =
                NormalizeVariantTypeOrNull(
                    variant.VariantType);

            if (type == null)
            {
                result.Errors.Add(
                    $"Variant '{name}' has invalid VariantType " +
                    $"'{variant.VariantType}'. Expected parent, " +
                    "subVariant or edition.");

                continue;
            }

            switch (type)
            {
                case "parent":
                    ValidateParentVariant(
                        variant,
                        result);
                    break;

                case "subVariant":
                    ValidateSubVariant(
                        variant,
                        variantsByName,
                        json,
                        result);
                    break;

                case "edition":
                    ValidateEditionVariant(
                        variant,
                        variantsByName,
                        result);
                    break;
            }
        }

        var parents =
            json.Variants
                .Where(x =>
                    string.Equals(
                        NormalizeVariantTypeOrNull(
                            x.VariantType),
                        "parent",
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

        foreach (var parent in parents)
        {
            var parentName =
                NormalizeNullable(
                    parent.VariantName);

            if (parentName == null)
                continue;

            var hasChild =
                json.Variants.Any(x =>
                    string.Equals(
                        NormalizeVariantTypeOrNull(
                            x.VariantType),
                        "subVariant",
                        StringComparison.OrdinalIgnoreCase)
                    &&
                    string.Equals(
                        x.ParentVariantName?.Trim(),
                        parentName,
                        StringComparison.Ordinal));

            if (!hasChild)
            {
                result.Errors.Add(
                    $"Parent variant '{parentName}' has no " +
                    "subVariant. Every parent must have at least one " +
                    "concrete child configuration.");
            }
        }

        // --------------------------------------------------------
        // Duplicate concrete configuration detection.
        //
        // Simple string key instead of a custom anonymous-type
        // comparer.
        // --------------------------------------------------------

        var concreteConfigurations =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var variant in json.Variants)
        {
            if (!string.Equals(
                    NormalizeVariantTypeOrNull(
                        variant.VariantType),
                    "subVariant",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var key =
                string.Join(
                    "\u001F",
                    variant.ParentVariantName?.Trim() ?? "",
                    variant.PowertrainRef?.Trim() ?? "",
                    variant.TransmissionRef?.Trim() ?? "",
                    variant.DrivetrainRef?.Trim() ?? "");

            if (!concreteConfigurations.Add(key))
            {
                result.Errors.Add(
                    $"Duplicate concrete configuration under parent " +
                    $"'{variant.ParentVariantName}': " +
                    $"PowertrainRef='{variant.PowertrainRef}', " +
                    $"TransmissionRef='{variant.TransmissionRef}', " +
                    $"DrivetrainRef='{variant.DrivetrainRef}'. " +
                    "Every supported concrete configuration must appear " +
                    "exactly once.");
            }
        }

        // --------------------------------------------------------
        // A ParentVariantName must always reference a parent.
        // --------------------------------------------------------

        foreach (var variant in json.Variants)
        {
            if (string.IsNullOrWhiteSpace(
                    variant.ParentVariantName))
            {
                continue;
            }

            var parentName =
                variant.ParentVariantName.Trim();

            if (!variantsByName.TryGetValue(
                    parentName,
                    out var matches))
            {
                result.Errors.Add(
                    $"Variant '{variant.VariantName}' references " +
                    $"ParentVariantName '{parentName}', but that " +
                    "variant does not exist.");

                continue;
            }

            if (matches.Count != 1)
                continue;

            var referenced =
                matches[0];

            if (!string.Equals(
                    NormalizeVariantTypeOrNull(
                        referenced.VariantType),
                    "parent",
                    StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add(
                    $"Variant '{variant.VariantName}' has " +
                    $"ParentVariantName '{parentName}', but " +
                    "that variant is not of type 'parent'.");
            }
        }
    }

    // ============================================================
    // PARENT VALIDATION
    // ============================================================

    private static void ValidateParentVariant(
        VariantDto variant,
        StageValidationResult result)
    {
        var name =
            variant.VariantName?.Trim() ?? "<unknown>";

        if (variant.ParentVariantName != null)
        {
            result.Errors.Add(
                $"Parent variant '{name}' must have " +
                "ParentVariantName = null.");
        }

        if (variant.BaseVariantName != null)
        {
            result.Errors.Add(
                $"Parent variant '{name}' must have " +
                "BaseVariantName = null.");
        }

        if (!string.IsNullOrWhiteSpace(
                variant.PowertrainRef)
            ||
            !string.IsNullOrWhiteSpace(
                variant.TransmissionRef)
            ||
            !string.IsNullOrWhiteSpace(
                variant.DrivetrainRef))
        {
            result.Errors.Add(
                $"Parent variant '{name}' contains configuration " +
                "references. Parent variants must have " +
                "PowertrainRef, TransmissionRef and DrivetrainRef " +
                "all set to null.");
        }

        if (variant.ExShowroomPrice.HasValue ||
            variant.KerbWeight.HasValue ||
            variant.SeatingCapacity.HasValue)
        {
            result.Errors.Add(
                $"Parent variant '{name}' contains configuration " +
                "values such as price, weight or seating capacity. " +
                "Parent variants must contain no concrete " +
                "configuration values.");
        }
    }

    // ============================================================
    // SUBVARIANT VALIDATION
    // ============================================================

    private static void ValidateSubVariant(
        VariantDto variant,
        Dictionary<string, List<VariantDto>> variantsByName,
        BrochureExtractionDto json,
        StageValidationResult result)
    {
        var name =
            variant.VariantName?.Trim() ?? "<unknown>";

        if (string.IsNullOrWhiteSpace(
                variant.ParentVariantName))
        {
            result.Errors.Add(
                $"SubVariant '{name}' must have " +
                "ParentVariantName.");
        }
        else
        {
            var parentName =
                variant.ParentVariantName.Trim();

            if (!variantsByName.TryGetValue(
                    parentName,
                    out var parentMatches))
            {
                result.Errors.Add(
                    $"SubVariant '{name}' references parent " +
                    $"'{parentName}', but that parent does not exist.");
            }
            else if (parentMatches.Count == 1 &&
                     !string.Equals(
                         NormalizeVariantTypeOrNull(
                             parentMatches[0].VariantType),
                         "parent",
                         StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add(
                    $"SubVariant '{name}' references " +
                    $"'{parentName}', but that variant is not " +
                    "a parent.");
            }
        }

        if (variant.BaseVariantName != null)
        {
            result.Errors.Add(
                $"SubVariant '{name}' must have " +
                "BaseVariantName = null.");
        }

        if (string.IsNullOrWhiteSpace(
                variant.PowertrainRef))
        {
            result.Errors.Add(
                $"SubVariant '{name}' does not have " +
                "PowertrainRef. Every concrete subVariant must " +
                "resolve to a powertrain.");
        }

        var hasConfiguration =
            !string.IsNullOrWhiteSpace(
                variant.PowertrainRef)
            ||
            !string.IsNullOrWhiteSpace(
                variant.TransmissionRef)
            ||
            !string.IsNullOrWhiteSpace(
                variant.DrivetrainRef);

        if (!hasConfiguration)
        {
            result.Errors.Add(
                $"SubVariant '{name}' has no configuration " +
                "references.");
        }

        if (!string.IsNullOrWhiteSpace(
                variant.PowertrainRef)
            &&
            !json.Powertrains.Any(x =>
                string.Equals(
                    x.PowertrainRef?.Trim(),
                    variant.PowertrainRef.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
        {
            result.Errors.Add(
                $"SubVariant '{name}' references unknown " +
                $"PowertrainRef '{variant.PowertrainRef}'.");
        }

        if (!string.IsNullOrWhiteSpace(
                variant.TransmissionRef)
            &&
            !json.Transmissions.Any(x =>
                string.Equals(
                    x.TransmissionRef?.Trim(),
                    variant.TransmissionRef.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
        {
            result.Errors.Add(
                $"SubVariant '{name}' references unknown " +
                $"TransmissionRef '{variant.TransmissionRef}'.");
        }

        // Drivetrain intentionally optional.
        if (!string.IsNullOrWhiteSpace(
                variant.DrivetrainRef)
            &&
            !json.Drivetrains.Any(x =>
                string.Equals(
                    x.DrivetrainRef?.Trim(),
                    variant.DrivetrainRef.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
        {
            result.Errors.Add(
                $"SubVariant '{name}' references unknown " +
                $"DrivetrainRef '{variant.DrivetrainRef}'.");
        }
    }

    // ============================================================
    // EDITION VALIDATION
    // ============================================================

    private static void ValidateEditionVariant(
        VariantDto variant,
        Dictionary<string, List<VariantDto>> variantsByName,
        StageValidationResult result)
    {
        var name =
            variant.VariantName?.Trim() ?? "<unknown>";

        if (string.IsNullOrWhiteSpace(
                variant.BaseVariantName))
        {
            result.Errors.Add(
                $"Edition '{name}' must have BaseVariantName. " +
                "An edition is only valid when the brochure establishes " +
                "that it is derived from an existing parent or subVariant.");

            return;
        }

        var baseName =
            variant.BaseVariantName.Trim();

        if (!variantsByName.TryGetValue(
                baseName,
                out var baseMatches))
        {
            result.Errors.Add(
                $"Edition '{name}' references BaseVariantName " +
                $"'{baseName}', but that variant does not exist.");

            return;
        }

        if (baseMatches.Count != 1)
            return;

        var baseVariant =
            baseMatches[0];

        var baseType =
            NormalizeVariantTypeOrNull(
                baseVariant.VariantType);

        if (baseType != "parent" &&
            baseType != "subVariant")
        {
            result.Errors.Add(
                $"Edition '{name}' has BaseVariantName " +
                $"'{baseName}', but the base is of type " +
                $"'{baseVariant.VariantType}'. An edition may only " +
                "derive from a parent or subVariant.");
        }

        if (!string.IsNullOrWhiteSpace(
                variant.PowertrainRef)
            ||
            !string.IsNullOrWhiteSpace(
                variant.TransmissionRef)
            ||
            !string.IsNullOrWhiteSpace(
                variant.DrivetrainRef))
        {
            result.Errors.Add(
                $"Edition '{name}' contains configuration references. " +
                "Derived editions must have PowertrainRef, " +
                "TransmissionRef and DrivetrainRef all set to null.");
        }

        if (variant.ExShowroomPrice.HasValue ||
            variant.KerbWeight.HasValue ||
            variant.SeatingCapacity.HasValue)
        {
            result.Errors.Add(
                $"Edition '{name}' contains concrete configuration " +
                "values. Derived editions must not duplicate the " +
                "base configuration.");
        }

        if (!string.IsNullOrWhiteSpace(
                variant.ParentVariantName))
        {
            var parentName =
                variant.ParentVariantName.Trim();

            if (!variantsByName.TryGetValue(
                    parentName,
                    out var parentMatches))
            {
                result.Errors.Add(
                    $"Edition '{name}' references " +
                    $"ParentVariantName '{parentName}', but that " +
                    "variant does not exist.");
            }
            else if (parentMatches.Count == 1)
            {
                var parent =
                    parentMatches[0];

                if (!string.Equals(
                        NormalizeVariantTypeOrNull(
                            parent.VariantType),
                        "parent",
                        StringComparison.OrdinalIgnoreCase))
                {
                    result.Errors.Add(
                        $"Edition '{name}' has ParentVariantName " +
                        $"'{parentName}', but that variant is not " +
                        "a parent.");
                }
                else if (baseType == "parent")
                {
                    if (!string.Equals(
                            parentName,
                            baseName,
                            StringComparison.Ordinal))
                    {
                        result.Errors.Add(
                            $"Edition '{name}' has " +
                            $"BaseVariantName '{baseName}' but " +
                            $"ParentVariantName '{parentName}'. " +
                            "When an edition is based directly on " +
                            "a parent, these must refer to the same " +
                            "parent.");
                    }
                }
                else if (baseType == "subVariant")
                {
                    if (!string.Equals(
                            parentName,
                            baseVariant.ParentVariantName?.Trim(),
                            StringComparison.Ordinal))
                    {
                        result.Errors.Add(
                            $"Edition '{name}' is based on " +
                            $"subVariant '{baseName}', whose " +
                            $"ParentVariantName is " +
                            $"'{baseVariant.ParentVariantName}', " +
                            $"but the edition specifies " +
                            $"ParentVariantName '{parentName}'.");
                    }
                }
            }
        }
    }

    // ============================================================
    // BUILD LOGICAL PARENT MAP
    // ============================================================

    private static Dictionary<int, string> BuildLogicalParentMap(
        BrochureExtractionDto json,
        List<string> errors)
    {
        var map =
            new Dictionary<int, string>();

        var variantsByName =
            json.Variants
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.VariantName))
                .GroupBy(
                    x => x.VariantName!.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList(),
                    StringComparer.OrdinalIgnoreCase);

        foreach (var variant in json.Variants)
        {
            var type =
                NormalizeVariantTypeOrNull(
                    variant.VariantType);

            if (type == "parent")
            {
                var name =
                    NormalizeNullable(
                        variant.VariantName);

                if (name != null)
                {
                    map[variant.IndexId] = name;
                }

                continue;
            }

            if (type == "subVariant")
            {
                var parentName =
                    NormalizeNullable(
                        variant.ParentVariantName);

                if (parentName != null)
                {
                    map[variant.IndexId] = parentName;
                }
            }
        }

        foreach (var variant in json.Variants)
        {
            if (!string.Equals(
                    NormalizeVariantTypeOrNull(
                        variant.VariantType),
                    "edition",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var explicitParent =
                NormalizeNullable(
                    variant.ParentVariantName);

            if (explicitParent != null)
            {
                map[variant.IndexId] =
                    explicitParent;

                continue;
            }

            var baseName =
                NormalizeNullable(
                    variant.BaseVariantName);

            if (baseName == null)
            {
                errors.Add(
                    $"Edition '{variant.VariantName}' cannot be assigned " +
                    "to an ImportVariantParent because BaseVariantName " +
                    "is missing.");

                continue;
            }

            if (!variantsByName.TryGetValue(
                    baseName,
                    out var baseMatches)
                ||
                baseMatches.Count != 1)
            {
                errors.Add(
                    $"Edition '{variant.VariantName}' cannot resolve " +
                    $"BaseVariantName '{baseName}' to a unique variant.");

                continue;
            }

            var baseVariant =
                baseMatches[0];

            var baseType =
                NormalizeVariantTypeOrNull(
                    baseVariant.VariantType);

            if (baseType == "parent")
            {
                var parentName =
                    NormalizeNullable(
                        baseVariant.VariantName);

                if (parentName != null)
                {
                    map[variant.IndexId] =
                        parentName;
                }
            }
            else if (baseType == "subVariant")
            {
                var parentName =
                    NormalizeNullable(
                        baseVariant.ParentVariantName);

                if (parentName != null)
                {
                    map[variant.IndexId] =
                        parentName;
                }
                else
                {
                    errors.Add(
                        $"Edition '{variant.VariantName}' is based on " +
                        $"subVariant '{baseName}', but that subVariant " +
                        "has no ParentVariantName.");
                }
            }
            else
            {
                errors.Add(
                    $"Edition '{variant.VariantName}' is based on " +
                    $"'{baseName}', which is not a parent or subVariant.");
            }
        }

        foreach (var variant in json.Variants)
        {
            if (!map.ContainsKey(
                    variant.IndexId))
            {
                errors.Add(
                    $"Variant '{variant.VariantName}' could not be " +
                    "assigned to a logical ImportVariantParent.");
            }
        }

        return map;
    }

    // ============================================================
    // VARIANT LOOKUP
    // ============================================================

    private static ImportVariant GetVariantByName(
        BrochureExtractionDto json,
        Dictionary<int, ImportVariant> variantMap,
        string variantName)
    {
        if (string.IsNullOrWhiteSpace(
                variantName))
        {
            throw new InvalidOperationException(
                "Variant name cannot be empty.");
        }

        var dto =
            json.Variants.FirstOrDefault(
                x =>
                    string.Equals(
                        x.VariantName?.Trim(),
                        variantName.Trim(),
                        StringComparison.OrdinalIgnoreCase));

        if (dto == null)
        {
            throw new InvalidOperationException(
                $"Variant '{variantName}' was not found.");
        }

        if (!variantMap.TryGetValue(
                dto.IndexId,
                out var variant))
        {
            throw new InvalidOperationException(
                $"Variant '{variantName}' has no staged database record.");
        }

        return variant;
    }

    // ============================================================
    // ENGINE -> POWERTRAIN
    // ============================================================

    private static string GetPowertrainRefForEngine(
        BrochureExtractionDto json,
        EngineDto engine)
    {
        var engineRef =
            NormalizeNullable(
                engine.EngineRef);

        if (engineRef == null)
        {
            throw new InvalidOperationException(
                "EngineRef cannot be empty.");
        }

        var matchingPowertrains =
            json.Powertrains
                .Where(x =>
                    string.Equals(
                        x.EngineRef?.Trim(),
                        engineRef,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (matchingPowertrains.Count == 0)
        {
            throw new InvalidOperationException(
                $"Engine '{engineRef}' is not referenced by " +
                "any Powertrain.EngineRef.");
        }

        if (matchingPowertrains.Count > 1)
        {
            throw new InvalidOperationException(
                $"Engine '{engineRef}' is referenced by multiple " +
                "Powertrains.");
        }

        var powertrainRef =
            NormalizeNullable(
                matchingPowertrains[0].PowertrainRef);

        if (powertrainRef == null)
        {
            throw new InvalidOperationException(
                $"Powertrain referencing Engine '{engineRef}' " +
                "has no PowertrainRef.");
        }

        return powertrainRef;
    }

    // ============================================================
    // INDEX ID VALIDATION
    // ============================================================

    private static void ValidateIndexIds(
        IEnumerable<int> indexIds,
        string entityName,
        StageValidationResult result)
    {
        var ids =
            indexIds.ToList();

        foreach (var group in ids
                     .GroupBy(x => x)
                     .Where(x => x.Count() > 1))
        {
            result.Errors.Add(
                $"{entityName} IndexId {group.Key} appears more than once.");
        }

        foreach (var indexId in ids)
        {
            if (indexId <= 0)
            {
                result.Errors.Add(
                    $"{entityName} has invalid IndexId {indexId}. " +
                    "IndexId must be greater than zero.");
            }
        }
    }

    // ============================================================
    // POWERTRAIN TYPE
    // ============================================================

    private static string NormalizePowertrainType(
        string value)
    {
        var normalized =
            NormalizePowertrainTypeOrNull(value);

        if (normalized == null)
        {
            throw new InvalidOperationException(
                $"Invalid PowertrainType '{value}'.");
        }

        return normalized;
    }

    private static string? NormalizePowertrainTypeOrNull(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized =
            value.Trim();

        if (normalized.Equals(
                "ICE",
                StringComparison.OrdinalIgnoreCase))
        {
            return "ICE";
        }

        if (normalized.Equals(
                "EV",
                StringComparison.OrdinalIgnoreCase))
        {
            return "EV";
        }

        if (normalized.Equals(
                "Hybrid",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Hybrid";
        }

        return null;
    }

    // ============================================================
    // VARIANT TYPE
    // ============================================================

    private static string NormalizeVariantType(
        string value)
    {
        var normalized =
            NormalizeVariantTypeOrNull(value);

        if (normalized == null)
        {
            throw new InvalidOperationException(
                $"Invalid VariantType '{value}'.");
        }

        return normalized;
    }

    private static string? NormalizeVariantTypeOrNull(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized =
            value.Trim();

        if (normalized.Equals(
                "parent",
                StringComparison.OrdinalIgnoreCase))
        {
            return "parent";
        }

        if (normalized.Equals(
                "subVariant",
                StringComparison.OrdinalIgnoreCase))
        {
            return "subVariant";
        }

        if (normalized.Equals(
                "edition",
                StringComparison.OrdinalIgnoreCase))
        {
            return "edition";
        }

        return null;
    }

    private static bool IsParentVariant(
        VariantDto variant)
    {
        return string.Equals(
            NormalizeVariantTypeOrNull(
                variant.VariantType),
            "parent",
            StringComparison.OrdinalIgnoreCase);
    }

    // ============================================================
    // STRING HELPERS
    // ============================================================

    private static string? NormalizeNullable(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    // ============================================================
    // BYTE / TINYINT
    // ============================================================

    private static byte? ToByte(
        int? value)
    {
        if (!value.HasValue)
            return null;

        if (value.Value < byte.MinValue ||
            value.Value > byte.MaxValue)
        {
            throw new InvalidOperationException(
                $"Value {value.Value} cannot be stored as TINYINT.");
        }

        return (byte)value.Value;
    }

    // ============================================================
    // PRIMARY DOCUMENT
    // ============================================================

    private async Task<int?> GetPrimaryDocumentIdAsync(
        int importBatchId,
        CancellationToken cancellationToken)
    {
        return await carsDbContext.ImportDocuments
            .Where(x =>
                x.ImportBatchId == importBatchId)
            .OrderBy(x =>
                x.ImportDocumentId)
            .Select(x =>
                (int?)x.ImportDocumentId)
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    // ============================================================
    // JSON OPTIONS
    // ============================================================

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    // ============================================================
    // VALIDATION RESULT
    // ============================================================

    private sealed class StageValidationResult
    {
        public List<string> Errors { get; } = new();

        public bool IsValid =>
            Errors.Count == 0;
    }

    // ============================================================
    // VALIDATION EXCEPTION
    // ============================================================

    public sealed class ImportBatchValidationException
        : Exception
    {
        public int ImportBatchId { get; }

        public IReadOnlyList<string> Errors { get; }

        public ImportBatchValidationException(
            int importBatchId,
            IEnumerable<string> errors)
            : base(
                $"Import batch {importBatchId} failed validation.")
        {
            ImportBatchId =
                importBatchId;

            Errors =
                errors
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();
        }
    }
}