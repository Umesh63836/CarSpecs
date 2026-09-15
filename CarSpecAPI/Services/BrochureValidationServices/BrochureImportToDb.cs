using CarSpecAPI.Data;
using CarSpecAPI.Data.Models.DataExtractionModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CarSpecAPI.Services.BrochureValidationServices
{
    public sealed class BrochureImportToDb
    {
        private readonly CarsDbContext carsDbContext;
        private readonly BrochureImportValidator _validator;

        public BrochureImportToDb(
            CarsDbContext db,
            BrochureImportValidator validator)
        {
            carsDbContext = db;
            _validator = validator;
        }

        //public async Task<int> ImportToStagingAsync(
        //    int importBatchId,
        //    int importDocumentId,
        //    string finalJson,
        //    CancellationToken cancellationToken = default)
        //{
        //    if (importBatchId <= 0)
        //    {
        //        throw new ArgumentOutOfRangeException(
        //            nameof(importBatchId));
        //    }

        //    if (importDocumentId <= 0)
        //    {
        //        throw new ArgumentOutOfRangeException(
        //            nameof(importDocumentId));
        //    }

        //    if (string.IsNullOrWhiteSpace(finalJson))
        //    {
        //        throw new BrochureImportException(
        //            "Final brochure JSON is empty.");
        //    }

        //    // ============================================================
        //    // 1. VERIFY BATCH / DOCUMENT
        //    // ============================================================

        //    var document =
        //        await carsDbContext.ImportDocuments
        //            .AsNoTracking()
        //            .FirstOrDefaultAsync(
        //                x =>
        //                    x.ImportDocumentId ==
        //                    importDocumentId,
        //                cancellationToken);

        //    if (document == null)
        //    {
        //        throw new BrochureImportException(
        //            $"ImportDocument {importDocumentId} was not found.");
        //    }

        //    if (document.ImportBatchId != importBatchId)
        //    {
        //        throw new BrochureImportException(
        //            $"ImportDocument {importDocumentId} does not " +
        //            $"belong to ImportBatch {importBatchId}.");
        //    }

        //    var batch =
        //        await carsDbContext.ImportBatches
        //            .FirstOrDefaultAsync(
        //                x =>
        //                    x.ImportBatchId ==
        //                    importBatchId,
        //                cancellationToken);

        //    if (batch == null)
        //    {
        //        throw new BrochureImportException(
        //            $"ImportBatch {importBatchId} was not found.");
        //    }

        //    // ============================================================
        //    // 2. DESERIALIZE FINAL JSON
        //    // ============================================================

        //    var options =
        //        new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        };

        //    BrochureExtractionDto? data;

        //    try
        //    {
        //        data =
        //            JsonSerializer.Deserialize<BrochureExtractionDto>(
        //                finalJson,
        //                options);
        //    }
        //    catch (JsonException ex)
        //    {
        //        throw new BrochureImportException(
        //            $"Invalid brochure JSON: {ex.Message}",
        //            ex);
        //    }

        //    if (data == null)
        //    {
        //        throw new BrochureImportException(
        //            "Brochure JSON deserialized to null.");
        //    }

        //    // ============================================================
        //    // 3. VALIDATE DTO
        //    // ============================================================

        //    var validation =
        //        await _validator.ValidateAsync(
        //            data,
        //            cancellationToken);

        //    if (!validation.IsValid)
        //    {
        //        throw new BrochureImportValidationException(
        //            validation.Errors);
        //    }

        //    // ============================================================
        //    // 4. PREVENT DUPLICATE IMPORT
        //    // ============================================================

        //    var existingImport =
        //        await carsDbContext.ImportRecords
        //            .AnyAsync(
        //                x =>
        //                    x.ImportBatchId ==
        //                    importBatchId &&
        //                    x.ImportDocumentId ==
        //                    importDocumentId &&
        //                    x.EntityType == "Model",
        //                cancellationToken);

        //    if (existingImport)
        //    {
        //        throw new BrochureImportException(
        //            $"Staging data already exists for " +
        //            $"ImportBatch {importBatchId} and " +
        //            $"ImportDocument {importDocumentId}.");
        //    }

        //    // ============================================================
        //    // 5. BEGIN TRANSACTION
        //    // ============================================================

        //    await using var transaction =
        //        await carsDbContext.Database.BeginTransactionAsync(
        //            cancellationToken);

        //    try
        //    {
        //        // ========================================================
        //        // 6. IMPORT RECORD
        //        // ========================================================

        //        var importRecord =
        //            new ImportRecord
        //            {
        //                ImportBatchId =
        //                    importBatchId,

        //                ImportDocumentId =
        //                    importDocumentId,

        //                EntityType =
        //                    "Model",

        //                EntityKey =
        //                    data.Vehicle!.ModelName,

        //                FieldName =
        //                    "Model",

        //                ExtractedValue =
        //                    data.Vehicle.ModelName,

        //                NormalizedValue =
        //                    data.Vehicle.ModelName,

        //                Confidence =
        //                    1.0m,

        //                Status =
        //                    "Pending",

        //                ReviewStatus =
        //                    "Pending"
        //            };

        //        carsDbContext.ImportRecords.Add(
        //            importRecord);

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 7. IMPORT MODEL
        //        // ========================================================

        //        var importModel =
        //            new ImportModel
        //            {
        //                ImportRecordId =
        //                    importRecord.ImportRecordId,

        //                ModelName =
        //                    data.Vehicle.ModelName!,

        //                BrandName =
        //                    data.Vehicle.Brand!,

        //                Category =
        //                    data.Vehicle.Category,

        //                BodyType =
        //                    data.Vehicle.BodyType
        //            };

        //        carsDbContext.ImportModels.Add(
        //            importModel);

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 8. PARENT VARIANTS
        //        // ========================================================

        //        var parentMap =
        //            new Dictionary<string, ImportVariantParent>(
        //                StringComparer.OrdinalIgnoreCase);

        //        foreach (var parent in data.Variants
        //                     .Where(v =>
        //                         string.Equals(
        //                             v.VariantType,
        //                             "parent",
        //                             StringComparison.OrdinalIgnoreCase)))
        //        {
        //            if (string.IsNullOrWhiteSpace(
        //                    parent.VariantName))
        //            {
        //                throw new BrochureImportException(
        //                    "Parent variant has empty variantName.");
        //            }

        //            if (parentMap.ContainsKey(
        //                    parent.VariantName))
        //            {
        //                throw new BrochureImportException(
        //                    $"Duplicate parent variant " +
        //                    $"'{parent.VariantName}'.");
        //            }

        //            var entity =
        //                new ImportVariantParent
        //                {
        //                    ImportModelId =
        //                        importModel.ImportModelId,

        //                    ParentVariantName =
        //                        parent.VariantName
        //                };

        //            carsDbContext.ImportVariantParents.Add(
        //                entity);

        //            parentMap.Add(
        //                parent.VariantName,
        //                entity);
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 9. POWERTRAINS
        //        // ========================================================

        //        var powertrainMap =
        //            new Dictionary<string, ImportPowertrain>(
        //                StringComparer.OrdinalIgnoreCase);

        //        foreach (var p in data.Powertrains)
        //        {
        //            if (string.IsNullOrWhiteSpace(
        //                    p.PowertrainRef))
        //            {
        //                throw new BrochureImportException(
        //                    "Powertrain has empty powertrainRef.");
        //            }

        //            var key =
        //                p.PowertrainRef.Trim();

        //            if (powertrainMap.ContainsKey(key))
        //            {
        //                throw new BrochureImportException(
        //                    $"Duplicate powertrainRef '{key}'.");
        //            }

        //            var entity =
        //                new ImportPowertrain
        //                {
        //                    ImportModelId =
        //                        importModel.ImportModelId,

        //                    PowertrainRef =
        //                        p.PowertrainRef,

        //                    PowertrainType =
        //                        p.PowertrainType,

        //                    BatteryCapacityKwh =
        //                        p.BatteryCapacityKWh,

        //                    CombinedMaxPower =
        //                        p.CombinedMaxPower,

        //                    CombinedMaxTorque =
        //                        p.CombinedMaxTorque,

        //                    CombinedMaxPowerRpm =
        //                        p.CombinedMaxPowerRPM,

        //                    CombinedMaxPowerRpmmin =
        //                        p.CombinedMaxPowerRPMMin,

        //                    CombinedMaxPowerRpmmax =
        //                        p.CombinedMaxPowerRPMMax,

        //                    CombinedMaxTorqueRpm =
        //                        p.CombinedMaxTorqueRPM,

        //                    CombinedMaxTorqueRpmmin =
        //                        p.CombinedMaxTorqueRPMMin,

        //                    CombinedMaxTorqueRpmmax =
        //                        p.CombinedMaxTorqueRPMMax
        //                };

        //            carsDbContext.ImportPowertrains.Add(
        //                entity);

        //            powertrainMap.Add(
        //                key,
        //                entity);
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 10. ENGINES
        //        // ========================================================

        //        var engineMap =
        //            new Dictionary<string, ImportEngine>(
        //                StringComparer.OrdinalIgnoreCase);

        //        foreach (var engine in data.Engines)
        //        {
        //            if (string.IsNullOrWhiteSpace(
        //                    engine.EngineRef))
        //            {
        //                throw new BrochureImportException(
        //                    "Engine has empty engineRef.");
        //            }

        //            var engineKey =
        //                engine.EngineRef.Trim();

        //            if (engineMap.ContainsKey(
        //                    engineKey))
        //            {
        //                throw new BrochureImportException(
        //                    $"Duplicate engineRef '{engineKey}'.");
        //            }

        //            var matchingPowertrain =
        //                data.Powertrains.FirstOrDefault(p =>
        //                    string.Equals(
        //                        p.EngineRef,
        //                        engine.EngineRef,
        //                        StringComparison.OrdinalIgnoreCase));

        //            if (matchingPowertrain == null)
        //            {
        //                throw new BrochureImportException(
        //                    $"No powertrain found for engineRef " +
        //                    $"'{engine.EngineRef}'.");
        //            }

        //            if (!powertrainMap.TryGetValue(
        //                    matchingPowertrain.PowertrainRef,
        //                    out var importPowertrain))
        //            {
        //                throw new BrochureImportException(
        //                    $"Powertrain " +
        //                    $"'{matchingPowertrain.PowertrainRef}' " +
        //                    $"was not staged for engine " +
        //                    $"'{engine.EngineRef}'.");
        //            }

        //            var entity =
        //                new ImportEngine
        //                {
        //                    ImportPowertrainId =
        //                        importPowertrain.ImportPowertrainId,

        //                    EngineRef =
        //                        engine.EngineRef,

        //                    EngineName =
        //                        engine.EngineName,

        //                    NumberOfCylinders =
        //                        engine.NumberOfCylinders.HasValue
        //                            ? (byte?)engine.NumberOfCylinders.Value
        //                            : null,

        //                    NumberOfValves =
        //                        engine.NumberOfValves.HasValue
        //                            ? (byte?)engine.NumberOfValves.Value
        //                            : null,

        //                    Displacement =
        //                        engine.Displacement,

        //                    IsTurbocharged =
        //                        engine.IsTurbocharged,

        //                    EmissionStandard =
        //                        engine.EmissionStandard,

        //                    Aspiration =
        //                        engine.Aspiration,

        //                    EngineType =
        //                        engine.EngineType
        //                };

        //            carsDbContext.ImportEngines.Add(
        //                entity);

        //            engineMap.Add(
        //                engineKey,
        //                entity);
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 11. ENGINE PERFORMANCES
        //        // ========================================================

        //        foreach (var engine in data.Engines)
        //        {
        //            if (!engineMap.TryGetValue(
        //                    engine.EngineRef,
        //                    out var importEngine))
        //            {
        //                throw new BrochureImportException(
        //                    $"Staged engine not found for " +
        //                    $"'{engine.EngineRef}'.");
        //            }

        //            if (engine.EnginePerformances == null)
        //                continue;

        //            foreach (var performance
        //                     in engine.EnginePerformances)
        //            {
        //                if (string.IsNullOrWhiteSpace(
        //                        performance.ModeName))
        //                {
        //                    throw new BrochureImportException(
        //                        $"Engine '{engine.EngineRef}' " +
        //                        "has performance without modeName.");
        //                }

        //                var entity =
        //                    new ImportEnginePerformance
        //                    {
        //                        ImportEngineId =
        //                            importEngine.ImportEngineId,

        //                        ModeName =
        //                            performance.ModeName,

        //                        FuelTypeId =
        //                            performance.FuelTypeId,

        //                        MaxPower =
        //                            performance.MaxPower,

        //                        MaxTorque =
        //                            performance.MaxTorque,

        //                        MaxPowerRpm =
        //                            performance.MaxPowerRPM,

        //                        MaxPowerRpmmin =
        //                            performance.MaxPowerRPMMin,

        //                        MaxPowerRpmmax =
        //                            performance.MaxPowerRPMMax,

        //                        MaxTorqueRpm =
        //                            performance.MaxTorqueRPM,

        //                        MaxTorqueRpmmin =
        //                            performance.MaxTorqueRPMMin,

        //                        MaxTorqueRpmmax =
        //                            performance.MaxTorqueRPMMax
        //                    };

        //                carsDbContext.ImportEnginePerformances.Add(
        //                    entity);
        //            }
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 12. MOTOR PERFORMANCES
        //        // ========================================================

        //        foreach (var powertrain in data.Powertrains)
        //        {
        //            if (!powertrainMap.TryGetValue(
        //                    powertrain.PowertrainRef,
        //                    out var importPowertrain))
        //            {
        //                throw new BrochureImportException(
        //                    $"Staged powertrain not found for " +
        //                    $"'{powertrain.PowertrainRef}'.");
        //            }

        //            if (powertrain.MotorPerformances == null)
        //                continue;

        //            foreach (var motor in
        //                     powertrain.MotorPerformances)
        //            {
        //                var entity =
        //                    new ImportMotorPerformance
        //                    {
        //                        ImportPowertrainId =
        //                            importPowertrain.ImportPowertrainId,

        //                        MotorName =
        //                            motor.MotorName,

        //                        MaxPower =
        //                            motor.MaxPower,

        //                        MaxTorque =
        //                            motor.MaxTorque,

        //                        MaxPowerRpm =
        //                            motor.MaxPowerRPM,

        //                        MaxPowerRpmmin =
        //                            motor.MaxPowerRPMMin,

        //                        MaxPowerRpmmax =
        //                            motor.MaxPowerRPMMax,

        //                        MaxTorqueRpm =
        //                            motor.MaxTorqueRPM,

        //                        MaxTorqueRpmmin =
        //                            motor.MaxTorqueRPMMin,

        //                        MaxTorqueRpmmax =
        //                            motor.MaxTorqueRPMMax
        //                    };

        //                carsDbContext.ImportMotorPerformances.Add(
        //                    entity);
        //            }
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 13. TRANSMISSIONS
        //        // ========================================================

        //        var transmissionMap =
        //            new Dictionary<string, ImportTransmission>(
        //                StringComparer.OrdinalIgnoreCase);

        //        foreach (var t in data.Transmissions)
        //        {
        //            if (string.IsNullOrWhiteSpace(
        //                    t.TransmissionRef))
        //            {
        //                throw new BrochureImportException(
        //                    "Transmission has empty transmissionRef.");
        //            }

        //            var key =
        //                t.TransmissionRef.Trim();

        //            if (transmissionMap.ContainsKey(key))
        //            {
        //                throw new BrochureImportException(
        //                    $"Duplicate transmissionRef '{key}'.");
        //            }

        //            var entity =
        //                new ImportTransmission
        //                {
        //                    ImportModelId =
        //                        importModel.ImportModelId,

        //                    TransmissionRef =
        //                        t.TransmissionRef,

        //                    TransmissionType =
        //                        NormalizeTransmissionType(
        //                            t.TransmissionType),

        //                    NumberOfGears =
        //                        t.NumberOfGears,

        //                    HasManualOverride =
        //                        t.HasManualOverride,

        //                    HasPaddleShifters =
        //                        t.HasPaddleShifters
        //                };

        //            carsDbContext.ImportTransmissions.Add(
        //                entity);

        //            transmissionMap.Add(
        //                key,
        //                entity);
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 14. DRIVETRAINS
        //        // ========================================================

        //        var drivetrainMap =
        //            new Dictionary<string, ImportDrivetrain>(
        //                StringComparer.OrdinalIgnoreCase);

        //        foreach (var d in data.Drivetrains)
        //        {
        //            if (string.IsNullOrWhiteSpace(
        //                    d.DrivetrainRef))
        //            {
        //                throw new BrochureImportException(
        //                    "Drivetrain has empty drivetrainRef.");
        //            }

        //            var key =
        //                d.DrivetrainRef.Trim();

        //            if (drivetrainMap.ContainsKey(key))
        //            {
        //                throw new BrochureImportException(
        //                    $"Duplicate drivetrainRef '{key}'.");
        //            }

        //            var entity =
        //                new ImportDrivetrain
        //                {
        //                    ImportModelId =
        //                        importModel.ImportModelId,

        //                    DrivetrainRef =
        //                        d.DrivetrainRef,

        //                    DrivetrainType =
        //                        d.DrivetrainType,

        //                    DifferentialType =
        //                        d.DifferentialType
        //                };

        //            carsDbContext.ImportDrivetrains.Add(
        //                entity);

        //            drivetrainMap.Add(
        //                key,
        //                entity);
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 15. CONCRETE VARIANTS
        //        // ========================================================

        //        var variantMap =
        //            new Dictionary<string, ImportVariant>(
        //                StringComparer.OrdinalIgnoreCase);

        //        var concreteVariants =
        //            data.Variants
        //                .Where(v =>
        //                    string.Equals(
        //                        v.VariantType,
        //                        "subVariant",
        //                        StringComparison.OrdinalIgnoreCase))
        //                .ToList();

        //        if (concreteVariants.Count == 0)
        //        {
        //            throw new BrochureImportException(
        //                "No concrete subVariants were found.");
        //        }

        //        foreach (var variant in concreteVariants)
        //        {
        //            var variantKey =
        //                variant.VariantName.Trim();

        //            if (variantMap.ContainsKey(
        //                    variantKey))
        //            {
        //                throw new BrochureImportException(
        //                    $"Duplicate concrete variant " +
        //                    $"'{variantKey}'.");
        //            }

        //            if (string.IsNullOrWhiteSpace(
        //                    variant.ParentVariantName))
        //            {
        //                throw new BrochureImportException(
        //                    $"Variant '{variant.VariantName}' " +
        //                    "has no parentVariantName.");
        //            }

        //            if (!parentMap.TryGetValue(
        //                    variant.ParentVariantName,
        //                    out var parent))
        //            {
        //                throw new BrochureImportException(
        //                    $"Parent '{variant.ParentVariantName}' " +
        //                    $"not found for variant " +
        //                    $"'{variant.VariantName}'.");
        //            }

        //            if (string.IsNullOrWhiteSpace(
        //                    variant.PowertrainRef))
        //            {
        //                throw new BrochureImportException(
        //                    $"Variant '{variant.VariantName}' " +
        //                    "has no powertrainRef.");
        //            }

        //            if (string.IsNullOrWhiteSpace(
        //                    variant.TransmissionRef))
        //            {
        //                throw new BrochureImportException(
        //                    $"Variant '{variant.VariantName}' " +
        //                    "has no transmissionRef.");
        //            }

        //            if (string.IsNullOrWhiteSpace(
        //                    variant.DrivetrainRef))
        //            {
        //                throw new BrochureImportException(
        //                    $"Variant '{variant.VariantName}' " +
        //                    "has no drivetrainRef.");
        //            }

        //            if (!powertrainMap.TryGetValue(
        //                    variant.PowertrainRef,
        //                    out var powertrain))
        //            {
        //                throw new BrochureImportException(
        //                    $"Powertrain '{variant.PowertrainRef}' " +
        //                    $"not found for " +
        //                    $"'{variant.VariantName}'.");
        //            }

        //            if (!transmissionMap.TryGetValue(
        //                    variant.TransmissionRef,
        //                    out var transmission))
        //            {
        //                throw new BrochureImportException(
        //                    $"Transmission " +
        //                    $"'{variant.TransmissionRef}' " +
        //                    $"not found for " +
        //                    $"'{variant.VariantName}'.");
        //            }

        //            if (!drivetrainMap.TryGetValue(
        //                    variant.DrivetrainRef,
        //                    out var drivetrain))
        //            {
        //                throw new BrochureImportException(
        //                    $"Drivetrain " +
        //                    $"'{variant.DrivetrainRef}' " +
        //                    $"not found for " +
        //                    $"'{variant.VariantName}'.");
        //            }

        //            var entity =
        //                new ImportVariant
        //                {
        //                    ImportVariantParentId =
        //                        parent.ImportVariantParentId,

        //                    VariantName =
        //                        variant.VariantName,

        //                    VariantType =
        //                        variant.VariantType,

        //                    ImportPowertrainId =
        //                        powertrain.ImportPowertrainId,

        //                    ImportTransmissionId =
        //                        transmission.ImportTransmissionId,

        //                    ImportDrivetrainId =
        //                        drivetrain.ImportDrivetrainId,

        //                    ExShowroomPrice =
        //                        variant.ExShowroomPrice,

        //                    KerbWeight =
        //                        variant.KerbWeight,

        //                    SeatingCapacity =
        //                        variant.SeatingCapacity
        //                };

        //            carsDbContext.ImportVariants.Add(
        //                entity);

        //            variantMap.Add(
        //                variantKey,
        //                entity);
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 16. FEATURES
        //        // ========================================================

        //        // Same FeatureId may legitimately occur more than once
        //        // with different values/applicability.

        //        var importFeatures =
        //            new List<(FeatureDto Dto, ImportFeature Entity)>();

        //        foreach (var feature in data.Features)
        //        {
        //            var entity =
        //                new ImportFeature
        //                {

        //                    FeatureId =
        //                        feature.FeatureId,

        //                    FeatureCode =
        //                        feature.FeatureCode,

        //                };

        //            carsDbContext.ImportFeatures.Add(
        //                entity);

        //            importFeatures.Add(
        //                (feature, entity));
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 17. FEATURE VALUE OPTIONS
        //        // ========================================================

        //        //foreach (var item in importFeatures)
        //        //{
        //        //    if (item.Dto.ValueOptionIds == null)
        //        //        continue;

        //        //    foreach (var optionId
        //        //             in item.Dto.ValueOptionIds
        //        //                 .Distinct())
        //        //    {
        //        //        carsDbContext.ImportFeatureValueOptions.Add(
        //        //            new ImportFeatureValueOption
        //        //            {
        //        //                ImportFeatureId =
        //        //                    item.Entity.ImportFeatureId,

        //        //                ValueOptionId =
        //        //                    optionId
        //        //            });
        //        //    }
        //        //}

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 18. FEATURE → VARIANT
        //        // ========================================================

        //        var featureVariantPairs =
        //            new HashSet<string>(
        //                StringComparer.OrdinalIgnoreCase);

        //        foreach (var item in importFeatures)
        //        {
        //            if (item.Dto.AppliesToVariants == null)
        //                continue;

        //            foreach (var variantName
        //                     in item.Dto.AppliesToVariants
        //                         .Where(x =>
        //                             !string.IsNullOrWhiteSpace(x))
        //                         .Distinct(
        //                             StringComparer.OrdinalIgnoreCase))
        //            {
        //                if (!variantMap.TryGetValue(
        //                        variantName,
        //                        out var variant))
        //                {
        //                    throw new BrochureImportException(
        //                        $"Feature '{item.Dto.FeatureCode}' " +
        //                        $"references unknown variant " +
        //                        $"'{variantName}'.");
        //                }

        //                var pairKey =
        //                    $"{item.Entity.ImportFeatureId}:" +
        //                    $"{variant.ImportVariantId}";

        //                if (!featureVariantPairs.Add(
        //                        pairKey))
        //                {
        //                    continue;
        //                }

        //                carsDbContext.ImportFeatureVariants.Add(
        //                    new ImportFeatureVariant
        //                    {
        //                        ImportFeatureId =
        //                            item.Entity.ImportFeatureId,

        //                        ImportVariantId =
        //                            variant.ImportVariantId
        //                    });
        //            }
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 19. SPECIFICATIONS
        //        // ========================================================

        //        var specifications =
        //            data.Specifications;

        //        var importSpecifications =
        //            new List<(
        //                SpecificationDto Dto,
        //                ImportSpecification Entity)>();

        //        foreach (var specification in specifications)
        //        {
        //            if (specification.AppliesToVariants == null ||
        //                specification.AppliesToVariants.Count == 0)
        //            {
        //                throw new BrochureImportException(
        //                    $"Specification " +
        //                    $"'{specification.SpecificationCode}' " +
        //                    "has no appliesToVariants.");
        //            }

        //            var entity =
        //                new ImportSpecification
        //                {
        //                    SpecificationId =
        //                        specification.SpecificationId,

        //                    SpecificationCode =
        //                        specification.SpecificationCode
        //                };

        //            carsDbContext.ImportSpecifications.Add(entity);

        //            importSpecifications.Add(
        //                (
        //                    specification,
        //                    entity));
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);


        //        // ========================================================
        //        // 20. SPECIFICATION → VARIANT
        //        // ========================================================

        //        var specificationVariantPairs =
        //            new HashSet<string>(
        //                StringComparer.OrdinalIgnoreCase);

        //        foreach (var item in importSpecifications)
        //        {
        //            foreach (var variantName
        //                     in item.Dto.AppliesToVariants
        //                         .Where(x =>
        //                             !string.IsNullOrWhiteSpace(x))
        //                         .Distinct(
        //                             StringComparer.OrdinalIgnoreCase))
        //            {
        //                if (!variantMap.TryGetValue(
        //                        variantName,
        //                        out var variant))
        //                {
        //                    throw new BrochureImportException(
        //                        $"Specification " +
        //                        $"'{item.Dto.SpecificationCode}' " +
        //                        $"references unknown variant " +
        //                        $"'{variantName}'.");
        //                }

        //                var pairKey =
        //                    $"{item.Entity.ImportSpecificationId}:" +
        //                    $"{variant.ImportVariantId}";

        //                if (!specificationVariantPairs.Add(
        //                        pairKey))
        //                {
        //                    continue;
        //                }

        //                carsDbContext.ImportSpecificationVariants.Add(
        //                    new ImportSpecificationVariant
        //                    {
        //                        ImportSpecificationId =
        //                            item.Entity.ImportSpecificationId,

        //                        ImportVariantId =
        //                            variant.ImportVariantId
        //                    });
        //            }
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 21. FUEL EFFICIENCY
        //        // ========================================================

        //        var fuelVariantPairs =
        //            new HashSet<string>(
        //                StringComparer.OrdinalIgnoreCase);

        //        foreach (var fuel in data.FuelEfficiencies)
        //        {
        //            var entity =
        //                new ImportFuelEfficiency
        //                {
        //                    FuelEfficiency =
        //                        fuel.FuelEfficiency,

        //                    FuelEfficiencyUnit =
        //                        fuel.FuelEfficiencyUnit,

        //                    SourceColumn =
        //                        fuel.SourceColumn,

        //                    PageNumber =
        //                        fuel.PageNumber,

        //                    Evidence =
        //                        fuel.Evidence,

        //                    Confidence =
        //                        fuel.Confidence
        //                };

        //            carsDbContext.ImportFuelEfficiencies.Add(
        //                entity);

        //            await carsDbContext.SaveChangesAsync(
        //                cancellationToken);

        //            if (fuel.AppliesToVariants == null)
        //            {
        //                throw new BrochureImportException(
        //                    "Fuel efficiency has null " +
        //                    "appliesToVariants.");
        //            }

        //            foreach (var variantName
        //                     in fuel.AppliesToVariants
        //                         .Where(x =>
        //                             !string.IsNullOrWhiteSpace(x))
        //                         .Distinct(
        //                             StringComparer.OrdinalIgnoreCase))
        //            {
        //                if (!variantMap.TryGetValue(
        //                        variantName,
        //                        out var variant))
        //                {
        //                    throw new BrochureImportException(
        //                        $"Fuel efficiency references " +
        //                        $"unknown variant " +
        //                        $"'{variantName}'.");
        //                }

        //                var pairKey =
        //                    $"{entity.ImportFuelEfficiencyId}:" +
        //                    $"{variant.ImportVariantId}";

        //                if (!fuelVariantPairs.Add(
        //                        pairKey))
        //                {
        //                    continue;
        //                }

        //                carsDbContext.ImportFuelEfficiencyVariants.Add(
        //                    new ImportFuelEfficiencyVariant
        //                    {
        //                        ImportFuelEfficiencyId =
        //                            entity.ImportFuelEfficiencyId,

        //                        ImportVariantId =
        //                            variant.ImportVariantId
        //                    });
        //            }
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 22. WARNINGS
        //        // ========================================================

        //        foreach (var warning in data.Warnings
        //                     ?? [])
        //        {
        //            if (string.IsNullOrWhiteSpace(
        //                    warning))
        //            {
        //                continue;
        //            }

        //            carsDbContext.ImportWarnings.Add(
        //                new ImportWarning
        //                {
        //                    ImportBatchId =
        //                        importBatchId,

        //                    ImportRecordId =
        //                        importRecord.ImportRecordId,

        //                    WarningText =
        //                        warning
        //                });
        //        }

        //        // Also persist validator warnings.
        //        foreach (var warning in validation.Warnings)
        //        {
        //            if (string.IsNullOrWhiteSpace(
        //                    warning))
        //            {
        //                continue;
        //            }

        //            carsDbContext.ImportWarnings.Add(
        //                new ImportWarning
        //                {
        //                    ImportBatchId =
        //                        importBatchId,

        //                    ImportRecordId =
        //                        importRecord.ImportRecordId,

        //                    WarningText =
        //                        warning
        //                });
        //        }

        //        await carsDbContext.SaveChangesAsync(
        //            cancellationToken);

        //        // ========================================================
        //        // 23. FINAL STAGING VALIDATION
        //        // ========================================================

        //        await ValidateStagedDataAsync(
        //            importModel.ImportModelId,
        //            importRecord.ImportRecordId,
        //            cancellationToken);

        //        // ========================================================
        //        // 24. COMMIT
        //        // ========================================================

        //        await transaction.CommitAsync(
        //            cancellationToken);

        //        return importRecord.ImportRecordId;
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync(
        //            cancellationToken);

        //        throw;
        //    }
        //}

        //// =================================================================
        //// TRANSMISSION NORMALIZATION
        //// =================================================================

        //private static string? NormalizeTransmissionType(
        //    string? value)
        //{
        //    if (string.IsNullOrWhiteSpace(
        //            value))
        //    {
        //        return value;
        //    }

        //    var normalized =
        //        value.Trim();

        //    // MT / 5MT / 6MT / Manual
        //    if (normalized.Equals(
        //            "MT",
        //            StringComparison.OrdinalIgnoreCase) ||
        //        normalized.Equals(
        //            "5MT",
        //            StringComparison.OrdinalIgnoreCase) ||
        //        normalized.Equals(
        //            "6MT",
        //            StringComparison.OrdinalIgnoreCase) ||
        //        normalized.Equals(
        //            "Manual",
        //            StringComparison.OrdinalIgnoreCase) ||
        //        normalized.Equals(
        //            "Manual Transmission",
        //            StringComparison.OrdinalIgnoreCase))
        //    {
        //        return "Manual";
        //    }

        //    // iMT / 6iMT / 7iMT / IMT
        //    //
        //    // Check this separately from MT so iMT
        //    // is NEVER normalized to Manual.
        //    if (normalized.Equals(
        //            "iMT",
        //            StringComparison.OrdinalIgnoreCase) ||
        //        normalized.Equals(
        //            "IMT",
        //            StringComparison.OrdinalIgnoreCase) ||
        //        normalized.Contains(
        //            "iMT",
        //            StringComparison.OrdinalIgnoreCase))
        //    {
        //        return "IMT";
        //    }

        //    return normalized;
        //}

        //// =================================================================
        //// FINAL STAGING VALIDATION
        //// =================================================================

        //private async Task ValidateStagedDataAsync(
        //    int importModelId,
        //    int importRecordId,
        //    CancellationToken cancellationToken)
        //{
        //    // -------------------------------------------------------------
        //    // MODEL
        //    // -------------------------------------------------------------

        //    var modelExists =
        //        await carsDbContext.ImportModels.AnyAsync(
        //            x =>
        //                x.ImportModelId ==
        //                    importModelId &&
        //                x.ImportRecordId ==
        //                    importRecordId,
        //            cancellationToken);

        //    if (!modelExists)
        //    {
        //        throw new BrochureImportException(
        //            "ImportModel was not created correctly.");
        //    }

        //    // -------------------------------------------------------------
        //    // PARENTS
        //    // -------------------------------------------------------------

        //    var parents =
        //        await carsDbContext.ImportVariantParents
        //            .Where(x =>
        //                x.ImportModelId ==
        //                importModelId)
        //            .Select(x =>
        //                new
        //                {
        //                    x.ImportVariantParentId,
        //                    x.ParentVariantName
        //                })
        //            .ToListAsync(
        //                cancellationToken);

        //    if (parents.Count == 0)
        //    {
        //        throw new BrochureImportException(
        //            "No parent variants were staged.");
        //    }

        //    var parentIds =
        //        parents
        //            .Select(x =>
        //                x.ImportVariantParentId)
        //            .ToHashSet();

        //    // -------------------------------------------------------------
        //    // VARIANTS
        //    // -------------------------------------------------------------

        //    var variants =
        //        await carsDbContext.ImportVariants
        //            .Where(x =>
        //                parentIds.Contains(
        //                    x.ImportVariantParentId))
        //            .Select(x =>
        //                new
        //                {
        //                    x.ImportVariantId,
        //                    x.ImportVariantParentId
        //                })
        //            .ToListAsync(
        //                cancellationToken);

        //    if (variants.Count == 0)
        //    {
        //        throw new BrochureImportException(
        //            "No concrete variants were staged.");
        //    }

        //    // -------------------------------------------------------------
        //    // EVERY PARENT MUST HAVE A CHILD
        //    // -------------------------------------------------------------

        //    var parentsWithChildren =
        //        variants
        //            .Select(x =>
        //                x.ImportVariantParentId)
        //            .ToHashSet();

        //    var parentsWithoutChildren =
        //        parents
        //            .Where(x =>
        //                !parentsWithChildren.Contains(
        //                    x.ImportVariantParentId))
        //            .ToList();

        //    if (parentsWithoutChildren.Count > 0)
        //    {
        //        throw new BrochureImportException(
        //            $"{parentsWithoutChildren.Count} parent " +
        //            "variant(s) have no concrete child.");
        //    }

        //    // -------------------------------------------------------------
        //    // ORPHAN VARIANTS
        //    // -------------------------------------------------------------

        //    var orphanVariants =
        //        await carsDbContext.ImportVariants
        //            .Where(x =>
        //                !carsDbContext.ImportVariantParents.Any(p =>
        //                    p.ImportVariantParentId ==
        //                        x.ImportVariantParentId &&
        //                    p.ImportModelId ==
        //                        importModelId))
        //            .CountAsync(
        //                cancellationToken);

        //    if (orphanVariants > 0)
        //    {
        //        throw new BrochureImportException(
        //            $"{orphanVariants} staged variants have " +
        //            "no valid parent for this import model.");
        //    }

        //    var variantIds =
        //        variants
        //            .Select(x =>
        //                x.ImportVariantId)
        //            .ToHashSet();

        //    // -------------------------------------------------------------
        //    // FEATURE MAPPINGS
        //    // -------------------------------------------------------------

        //    var invalidFeatureMappings =
        //        await carsDbContext.ImportFeatureVariants
        //            .Where(x =>
        //                !variantIds.Contains(
        //                    x.ImportVariantId))
        //            .CountAsync(
        //                cancellationToken);

        //    if (invalidFeatureMappings > 0)
        //    {
        //        throw new BrochureImportException(
        //            "Invalid feature → variant mappings detected.");
        //    }

        //    var featureIds =
        //        await carsDbContext.ImportFeatures
        //            .Select(x =>
        //                x.ImportFeatureId)
        //            .ToListAsync(
        //                cancellationToken);

        //    var invalidFeatureMappingsForModel =
        //        await carsDbContext.ImportFeatureVariants
        //            .Where(x =>
        //                featureIds.Contains(
        //                    x.ImportFeatureId) &&
        //                !variantIds.Contains(
        //                    x.ImportVariantId))
        //            .CountAsync(
        //                cancellationToken);

        //    if (invalidFeatureMappingsForModel > 0)
        //    {
        //        throw new BrochureImportException(
        //            "Feature mappings contain variants " +
        //            "outside the current import model.");
        //    }

        //    // -------------------------------------------------------------
        //    // SPECIFICATION MAPPINGS
        //    // -------------------------------------------------------------

        //    var specificationIds =
        //        await carsDbContext.ImportSpecifications
        //            .Select(x =>
        //                x.ImportSpecificationId)
        //            .ToListAsync(
        //                cancellationToken);

        //    var invalidSpecificationMappings =
        //        await carsDbContext.ImportSpecificationVariants
        //            .Where(x =>
        //                specificationIds.Contains(
        //                    x.ImportSpecificationId) &&
        //                !variantIds.Contains(
        //                    x.ImportVariantId))
        //            .CountAsync(
        //                cancellationToken);

        //    if (invalidSpecificationMappings > 0)
        //    {
        //        throw new BrochureImportException(
        //            "Invalid specification → variant mappings detected.");
        //    }

        //    // -------------------------------------------------------------
        //    // FUEL EFFICIENCY MAPPINGS
        //    // -------------------------------------------------------------

        //    var fuelIds =
        //        await carsDbContext.ImportFuelEfficiencies
        //            .Select(x =>
        //                x.ImportFuelEfficiencyId)
        //            .ToListAsync(
        //                cancellationToken);

        //    var invalidFuelMappings =
        //        await carsDbContext.ImportFuelEfficiencyVariants
        //            .Where(x =>
        //                fuelIds.Contains(
        //                    x.ImportFuelEfficiencyId) &&
        //                !variantIds.Contains(
        //                    x.ImportVariantId))
        //            .CountAsync(
        //                cancellationToken);

        //    if (invalidFuelMappings > 0)
        //    {
        //        throw new BrochureImportException(
        //            "Invalid fuel-efficiency → variant mappings detected.");
        //    }
        //}
    }
}