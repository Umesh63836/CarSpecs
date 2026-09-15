using CarSpecAPI.Data.Models.DataExtractionModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static CarSpecAPI.Services.AdministrationServices.StagedDataService;

namespace CarSpecAPI.Services.AdministrationServices
{
    public sealed class StagedDataService : IStagedDataService
    {
        private readonly CarsDbContext _context;
        private readonly ILogger<StagedDataService> _logger;

        public StagedDataService(
            CarsDbContext context,
            ILogger<StagedDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private const string StagedStatus = "Staged";

        // ============================================================
        // COMMON HELPERS
        // ============================================================

        private static string? Clean(string? value)
        {
            if (value == null)
                return null;

            var result = value.Trim();

            return string.IsNullOrWhiteSpace(result)
                ? null
                : result;
        }

        private static string Required(
            string? value,
            string fieldName)
        {
            var result = Clean(value);

            if (result == null)
            {
                throw new ArgumentException(
                    $"{fieldName} is required.");
            }

            return result;
        }

        private static void ValidateId(
            int id,
            string parameterName)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    id,
                    $"{parameterName} must be greater than zero.");
            }
        }

        private async Task<ImportBatch> GetStagedBatchForModelAsync(
            int importModelId,
            CancellationToken cancellationToken)
        {
            ValidateId(importModelId, nameof(importModelId));

            var model = await _context.ImportModels
                .AsNoTracking()
                .Include(x => x.ImportRecord)
                .FirstOrDefaultAsync(
                    x => x.ImportModelId == importModelId,
                    cancellationToken);

            if (model == null)
            {
                throw new KeyNotFoundException(
                    $"Staged model {importModelId} was not found.");
            }

            if (model.ImportRecord == null)
            {
                throw new InvalidOperationException(
                    $"Staged model {importModelId} is not associated with an import record.");
            }

            var batch = await _context.ImportBatches
                .FirstOrDefaultAsync(
                    x => x.ImportBatchId == model.ImportRecord.ImportBatchId,
                    cancellationToken);

            if (batch == null)
            {
                throw new InvalidOperationException(
                    $"Import batch {model.ImportRecord.ImportBatchId} associated with model {importModelId} was not found.");
            }

            if (!string.Equals(
                    batch.Status,
                    StagedStatus,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Import batch {batch.ImportBatchId} is not staged. Current status: '{batch.Status}'.");
            }

            return batch;
        }

        private async Task<ImportBatch> GetStagedBatchAsync(
            int importBatchId,
            CancellationToken cancellationToken)
        {
            ValidateId(importBatchId, nameof(importBatchId));

            var batch = await _context.ImportBatches
                .FirstOrDefaultAsync(
                    x => x.ImportBatchId == importBatchId,
                    cancellationToken);

            if (batch == null)
            {
                throw new KeyNotFoundException(
                    $"Import batch {importBatchId} was not found.");
            }

            if (!string.Equals(
                    batch.Status,
                    StagedStatus,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Import batch {importBatchId} is not staged. Current status: '{batch.Status}'.");
            }

            return batch;
        }

        private async Task<int> GetModelIdForVariantAsync(
            int importVariantId,
            CancellationToken cancellationToken)
        {
            ValidateId(importVariantId, nameof(importVariantId));

            var modelId = await _context.ImportVariants
                .Where(v => v.ImportVariantId == importVariantId)
                .Select(v =>
                    (int?)v.ImportVariantParent.ImportModelId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!modelId.HasValue)
            {
                throw new KeyNotFoundException(
                    $"Staged variant {importVariantId} was not found or has no parent model.");
            }

            return modelId.Value;
        }

        private async Task<int> GetModelIdForPowertrainAsync(
            int importPowertrainId,
            CancellationToken cancellationToken)
        {
            ValidateId(importPowertrainId, nameof(importPowertrainId));

            var modelId = await _context.ImportPowertrains
                .Where(x => x.ImportPowertrainId == importPowertrainId)
                .Select(x => (int?)x.ImportModelId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!modelId.HasValue)
            {
                throw new KeyNotFoundException(
                    $"Staged powertrain {importPowertrainId} was not found.");
            }

            return modelId.Value;
        }

        private async Task<int> GetModelIdForTransmissionAsync(
            int importTransmissionId,
            CancellationToken cancellationToken)
        {
            ValidateId(importTransmissionId, nameof(importTransmissionId));

            var modelId = await _context.ImportTransmissions
                .Where(x => x.ImportTransmissionId == importTransmissionId)
                .Select(x => (int?)x.ImportModelId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!modelId.HasValue)
            {
                throw new KeyNotFoundException(
                    $"Staged transmission {importTransmissionId} was not found.");
            }

            return modelId.Value;
        }

        private async Task<int> GetModelIdForDrivetrainAsync(
            int importDrivetrainId,
            CancellationToken cancellationToken)
        {
            ValidateId(importDrivetrainId, nameof(importDrivetrainId));

            var modelId = await _context.ImportDrivetrains
                .Where(x => x.ImportDrivetrainId == importDrivetrainId)
                .Select(x => (int?)x.ImportModelId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!modelId.HasValue)
            {
                throw new KeyNotFoundException(
                    $"Staged drivetrain {importDrivetrainId} was not found.");
            }

            return modelId.Value;
        }

        // ============================================================
        // MODEL LIST
        // ============================================================

        public async Task<PaginatedStagedModelResponseDto> GetModelsAsync(
            int pageNumber,
            int pageSize,
            string? search,
            CancellationToken cancellationToken)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pageNumber),
                    "Page number must be at least 1.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pageSize),
                    "Page size must be between 1 and 100.");
            }

            search = Clean(search);

            var query =
                from model in _context.ImportModels.AsNoTracking()
                join record in _context.ImportRecords.AsNoTracking()
                    on model.ImportRecordId equals record.ImportRecordId
                join batch in _context.ImportBatches.AsNoTracking()
                    on record.ImportBatchId equals batch.ImportBatchId
                where batch.Status == StagedStatus
                select new
                {
                    Model = model,
                    Batch = batch
                };

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Model.BrandName.Contains(search) ||
                    x.Model.ModelName.Contains(search));
            }

            var totalRecords =
                await query.CountAsync(cancellationToken);

            var totalPages =
                totalRecords == 0
                    ? 0
                    : (int)Math.Ceiling(
                        totalRecords / (double)pageSize);

            var models = await query
                .OrderByDescending(x => x.Batch.CompletedAt)
                .ThenBy(x => x.Model.BrandName)
                .ThenBy(x => x.Model.ModelName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StagedModelListDto
                {
                    ImportModelId =
                        x.Model.ImportModelId,

                    BrandName =
                        x.Model.BrandName ?? string.Empty,

                    ModelName =
                        x.Model.ModelName ?? string.Empty,

                    Category =
                        x.Model.Category,

                    BodyType =
                        x.Model.BodyType,

                    ModelImageUrl =
                        x.Model.ModelImageUrl,

                    ImportBatchId =
                        x.Batch.ImportBatchId,

                    StagedAt =
                        x.Batch.CompletedAt,

                    ParentVariantCount =
                        _context.ImportVariantParents
                            .Count(p =>
                                p.ImportModelId ==
                                x.Model.ImportModelId),

                    VariantCount =
                        _context.ImportVariants
                            .Count(v =>
                                _context.ImportVariantParents
                                    .Any(p =>
                                        p.ImportVariantParentId ==
                                            v.ImportVariantParentId &&
                                        p.ImportModelId ==
                                            x.Model.ImportModelId)),

                    WarningCount =
                        _context.ImportWarnings
                            .Count(w =>
                                w.ImportBatchId ==
                                x.Batch.ImportBatchId)
                })
                .ToListAsync(cancellationToken);

            return new PaginatedStagedModelResponseDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages,
                Models = models
            };
        }

        // ============================================================
        // COMPLETE MODEL
        // ============================================================

        public async Task<StagedModelDetailDto> GetModelAsync(
            int importModelId,
            CancellationToken cancellationToken)
        {
            var batch = await GetStagedBatchForModelAsync(
                importModelId,
                cancellationToken);

            var model = await _context.ImportModels
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.ImportModelId == importModelId,
                    cancellationToken);

            if (model == null)
            {
                throw new KeyNotFoundException(
                    $"Staged model {importModelId} was not found.");
            }

            // ----------------------------
            // Dimensions
            // ----------------------------

            var dimensions = await _context.ImportModelDimensions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.ImportModelId == importModelId,
                    cancellationToken);

            // ----------------------------
            // Parents
            // ----------------------------

            var parents = await _context.ImportVariantParents
                .AsNoTracking()
                .Where(x => x.ImportModelId == importModelId)
                .OrderBy(x => x.ImportVariantParentId)
                .ToListAsync(cancellationToken);

            // ----------------------------
            // Variants
            // ----------------------------

            var variants = await _context.ImportVariants
                .AsNoTracking()
                .Where(x =>
                    _context.ImportVariantParents.Any(p =>
                        p.ImportVariantParentId ==
                            x.ImportVariantParentId &&
                        p.ImportModelId ==
                            importModelId))
                .OrderBy(x => x.ImportVariantParentId)
                .ThenBy(x => x.ImportVariantId)
                .ToListAsync(cancellationToken);

            var variantIds =
                variants
                    .Select(x => x.ImportVariantId)
                    .ToList();

            // ----------------------------
            // Powertrains
            // ----------------------------

            var powertrainIds =
                variants
                    .Where(x => x.ImportPowertrainId.HasValue)
                    .Select(x => x.ImportPowertrainId!.Value)
                    .Distinct()
                    .ToList();

            var powertrains =
                await _context.ImportPowertrains
                    .AsNoTracking()
                    .Where(x =>
                        x.ImportModelId == importModelId &&
                        powertrainIds.Contains(
                            x.ImportPowertrainId))
                    .ToListAsync(cancellationToken);

            // ----------------------------
            // Transmissions
            // ----------------------------

            var transmissionIds =
                variants
                    .Where(x => x.ImportTransmissionId.HasValue)
                    .Select(x => x.ImportTransmissionId!.Value)
                    .Distinct()
                    .ToList();

            var transmissions =
                await _context.ImportTransmissions
                    .AsNoTracking()
                    .Where(x =>
                        x.ImportModelId == importModelId &&
                        transmissionIds.Contains(
                            x.ImportTransmissionId))
                    .ToListAsync(cancellationToken);

            // ----------------------------
            // Drivetrains
            // ----------------------------

            var drivetrainIds =
                variants
                    .Where(x => x.ImportDrivetrainId.HasValue)
                    .Select(x => x.ImportDrivetrainId!.Value)
                    .Distinct()
                    .ToList();

            var drivetrains =
                await _context.ImportDrivetrains
                    .AsNoTracking()
                    .Where(x =>
                        x.ImportModelId == importModelId &&
                        drivetrainIds.Contains(
                            x.ImportDrivetrainId))
                    .ToListAsync(cancellationToken);

            // ----------------------------
            // Engines
            // ----------------------------

            var engines =
                await _context.ImportEngines
                    .AsNoTracking()
                    .Where(x =>
                        powertrainIds.Contains(
                            x.ImportPowertrainId))
                    .ToListAsync(cancellationToken);

            var engineIds =
                engines
                    .Select(x => x.ImportEngineId)
                    .ToList();

            // ----------------------------
            // Engine Performance
            // ----------------------------

            var enginePerformances =
                await _context.ImportEnginePerformances
                    .AsNoTracking()
                    .Where(x =>
                        engineIds.Contains(
                            x.ImportEngineId))
                    .OrderBy(x => x.ImportEnginePerformanceId)
                    .ToListAsync(cancellationToken);

            // ----------------------------
            // Motor Performance
            // ----------------------------

            var motorPerformances =
                await _context.ImportMotorPerformances
                    .AsNoTracking()
                    .Where(x =>
                        powertrainIds.Contains(
                            x.ImportPowertrainId))
                    .OrderBy(x => x.ImportMotorPerformanceId)
                    .ToListAsync(cancellationToken);

            // ----------------------------
            // Features
            // ----------------------------

            var featureVariants =
                await _context.ImportFeatureVariants
                    .AsNoTracking()
                    .Where(x =>
                        variantIds.Contains(
                            x.ImportVariantId))
                    .OrderBy(x => x.ImportFeatureVariantId)
                    .ToListAsync(cancellationToken);

            var importFeatureIds =
                featureVariants
                    .Select(x => x.ImportFeatureId)
                    .Distinct()
                    .ToList();

            var importFeatures =
                await _context.ImportFeatures
                    .AsNoTracking()
                    .Where(x =>
                        importFeatureIds.Contains(
                            x.ImportFeatureId))
                    .ToListAsync(cancellationToken);

            var productionFeatureIds =
                importFeatures
                    .Where(x => x.FeatureId.HasValue)
                    .Select(x => x.FeatureId!.Value)
                    .Distinct()
                    .ToList();

            var productionFeatures =
                await _context.Features
                    .AsNoTracking()
                    .Where(x =>
                        productionFeatureIds.Contains(
                            x.FeatureId))
                    .ToListAsync(cancellationToken);

            var featureVariantIds =
                featureVariants
                    .Select(x => x.ImportFeatureVariantId)
                    .ToList();

            var optionRows =
                await _context.ImportFeatureValueOptions
                    .AsNoTracking()
                    .Where(x =>
                        featureVariantIds.Contains(
                            x.ImportFeatureVariantId))
                    .ToListAsync(cancellationToken);

            var optionIds =
                optionRows
                    .Select(x => x.ValueOptionId)
                    .Distinct()
                    .ToList();

            var valueOptions =
                await _context.FeatureValueOptions
                    .AsNoTracking()
                    .Where(x =>
                        optionIds.Contains(
                            x.FeatureValueOptionId))
                    .ToListAsync(cancellationToken);

            // ----------------------------
            // Specifications
            // ----------------------------

            var specificationVariants =
                await _context.ImportSpecificationVariants
                    .AsNoTracking()
                    .Where(x =>
                        variantIds.Contains(
                            x.ImportVariantId))
                    .OrderBy(x => x.ImportSpecificationVariantId)
                    .ToListAsync(cancellationToken);

            var importSpecificationIds =
                specificationVariants
                    .Select(x => x.ImportSpecificationId)
                    .Distinct()
                    .ToList();

            var importSpecifications =
                await _context.ImportSpecifications
                    .AsNoTracking()
                    .Where(x =>
                        importSpecificationIds.Contains(
                            x.ImportSpecificationId))
                    .ToListAsync(cancellationToken);

            var productionSpecificationIds =
                importSpecifications
                    .Where(x => x.SpecificationId.HasValue)
                    .Select(x => x.SpecificationId!.Value)
                    .Distinct()
                    .ToList();

            var productionSpecifications =
                await _context.Specifications
                    .AsNoTracking()
                    .Where(x =>
                        productionSpecificationIds.Contains(
                            x.SpecificationId))
                    .ToListAsync(cancellationToken);

            // ----------------------------
            // Fuel efficiency
            // ----------------------------

            var fuelEfficiencyVariants =
                await _context.ImportFuelEfficiencyVariants
                    .AsNoTracking()
                    .Where(x =>
                        variantIds.Contains(
                            x.ImportVariantId))
                    .OrderBy(x => x.ImportFuelEfficiencyVariantId)
                    .ToListAsync(cancellationToken);

            var fuelEfficiencyIds =
                fuelEfficiencyVariants
                    .Select(x => x.ImportFuelEfficiencyId)
                    .Distinct()
                    .ToList();

            var fuelEfficiencies =
                await _context.ImportFuelEfficiencies
                    .AsNoTracking()
                    .Where(x =>
                        fuelEfficiencyIds.Contains(
                            x.ImportFuelEfficiencyId))
                    .ToListAsync(cancellationToken);

            // ----------------------------
            // Warranties
            // ----------------------------

            var warranties =
                await _context.ImportWarranties
                    .AsNoTracking()
                    .Where(x =>
                        x.ImportModelId == importModelId)
                    .OrderBy(x => x.ImportWarrantyId)
                    .ToListAsync(cancellationToken);

            // ----------------------------
            // Warnings
            // ----------------------------

            var warnings =
                await _context.ImportWarnings
                    .AsNoTracking()
                    .Where(x =>
                        x.ImportBatchId ==
                        batch.ImportBatchId)
                    .OrderBy(x => x.ImportWarningId)
                    .Select(x => new StagedWarningDto
                    {
                        ImportWarningId =
                            x.ImportWarningId,

                        ImportRecordId =
                            x.ImportRecordId,

                        WarningText =
                            x.WarningText,

                        Status =
                            x.Status,

                        ReviewedBy =
                            x.ReviewedBy,

                        ReviewedAt =
                            x.ReviewedAt,

                        Notes =
                            x.Notes
                    })
                    .ToListAsync(cancellationToken);

            // ----------------------------
            // Assemble hierarchy
            // ----------------------------

            var parentDtos =
                new List<StagedVariantParentDto>();

            foreach (var parent in parents)
            {
                var parentDto =
                    new StagedVariantParentDto
                    {
                        ImportVariantParentId =
                            parent.ImportVariantParentId,

                        ParentVariantName =
                            parent.ParentVariantName,

                        SubVariants = []
                    };

                var parentVariants =
                    variants
                        .Where(x =>
                            x.ImportVariantParentId ==
                            parent.ImportVariantParentId)
                        .ToList();

                foreach (var variant in parentVariants)
                {
                    parentDto.SubVariants.Add(
                        BuildVariantDto(
                            variant,
                            powertrains,
                            transmissions,
                            drivetrains,
                            engines,
                            enginePerformances,
                            motorPerformances,
                            featureVariants,
                            importFeatures,
                            productionFeatures,
                            optionRows,
                            valueOptions,
                            specificationVariants,
                            importSpecifications,
                            productionSpecifications,
                            fuelEfficiencyVariants,
                            fuelEfficiencies,
                            warranties));
                }

                parentDtos.Add(parentDto);
            }

            return new StagedModelDetailDto
            {
                ImportModelId =
                    model.ImportModelId,

                ImportBatchId =
                    batch.ImportBatchId,

                BrandName =
                    model.BrandName ?? string.Empty,

                ModelName =
                    model.ModelName ?? string.Empty,

                Category =
                    model.Category,

                BodyType =
                    model.BodyType,

                ModelImageUrl =
                    model.ModelImageUrl,

                Dimensions =
                    dimensions == null
                        ? new StagedModelDimensionsDto()
                        : MapDimensions(dimensions),

                ParentVariants =
                    parentDtos,

                ModelWarranties =
                    warranties
                        .Where(x =>
                            !x.ImportVariantId.HasValue)
                        .Select(MapWarranty)
                        .ToList(),

                Warnings =
                    warnings
            };
        }

        // ============================================================
        // VARIANT ASSEMBLY
        // ============================================================

        private StagedVariantDetailDto BuildVariantDto(
            ImportVariant variant,
            IReadOnlyCollection<ImportPowertrain> powertrains,
            IReadOnlyCollection<ImportTransmission> transmissions,
            IReadOnlyCollection<ImportDrivetrain> drivetrains,
            IReadOnlyCollection<ImportEngine> engines,
            IReadOnlyCollection<ImportEnginePerformance> enginePerformances,
            IReadOnlyCollection<ImportMotorPerformance> motorPerformances,
            IReadOnlyCollection<ImportFeatureVariant> featureVariants,
            IReadOnlyCollection<ImportFeature> importFeatures,
            IReadOnlyCollection<Feature> productionFeatures,
            IReadOnlyCollection<ImportFeatureValueOption> optionRows,
            IReadOnlyCollection<FeatureValueOption> valueOptions,
            IReadOnlyCollection<ImportSpecificationVariant> specificationVariants,
            IReadOnlyCollection<ImportSpecification> importSpecifications,
            IReadOnlyCollection<Specification> productionSpecifications,
            IReadOnlyCollection<ImportFuelEfficiencyVariant> fuelEfficiencyVariants,
            IReadOnlyCollection<ImportFuelEfficiency> fuelEfficiencies,
            IReadOnlyCollection<ImportWarranty> warranties)
        {
            ImportPowertrain? powertrain = null;

            if (variant.ImportPowertrainId.HasValue)
            {
                powertrain =
                    powertrains.FirstOrDefault(x =>
                        x.ImportPowertrainId ==
                        variant.ImportPowertrainId.Value);
            }

            // Safety fallback using Ref.
            if (powertrain == null &&
                !string.IsNullOrWhiteSpace(
                    variant.PowertrainRef))
            {
                powertrain =
                    powertrains.FirstOrDefault(x =>
                        string.Equals(
                            x.PowertrainRef,
                            variant.PowertrainRef,
                            StringComparison.Ordinal));
            }

            ImportTransmission? transmission = null;

            if (variant.ImportTransmissionId.HasValue)
            {
                transmission =
                    transmissions.FirstOrDefault(x =>
                        x.ImportTransmissionId ==
                        variant.ImportTransmissionId.Value);
            }

            if (transmission == null &&
                !string.IsNullOrWhiteSpace(
                    variant.TransmissionRef))
            {
                transmission =
                    transmissions.FirstOrDefault(x =>
                        string.Equals(
                            x.TransmissionRef,
                            variant.TransmissionRef,
                            StringComparison.Ordinal));
            }

            ImportDrivetrain? drivetrain = null;

            if (variant.ImportDrivetrainId.HasValue)
            {
                drivetrain =
                    drivetrains.FirstOrDefault(x =>
                        x.ImportDrivetrainId ==
                        variant.ImportDrivetrainId.Value);
            }

            if (drivetrain == null &&
                !string.IsNullOrWhiteSpace(
                    variant.DrivetrainRef))
            {
                drivetrain =
                    drivetrains.FirstOrDefault(x =>
                        string.Equals(
                            x.DrivetrainRef,
                            variant.DrivetrainRef,
                            StringComparison.Ordinal));
            }

            ImportEngine? engine = null;

            if (powertrain != null &&
                !string.IsNullOrWhiteSpace(
                    powertrain.EngineRef))
            {
                engine =
                    engines.FirstOrDefault(x =>
                        x.ImportPowertrainId ==
                            powertrain.ImportPowertrainId &&
                        string.Equals(
                            x.EngineRef,
                            powertrain.EngineRef,
                            StringComparison.Ordinal));
            }

            var enginePerformanceDtos =
                engine == null
                    ? []
                    : enginePerformances
                        .Where(x =>
                            x.ImportEngineId ==
                            engine.ImportEngineId)
                        .Select(MapEnginePerformance)
                        .ToList();

            var motorPerformanceDtos =
                powertrain == null
                    ? []
                    : motorPerformances
                        .Where(x =>
                            x.ImportPowertrainId ==
                            powertrain.ImportPowertrainId)
                        .Select(MapMotorPerformance)
                        .ToList();

            var featureDtos =
                featureVariants
                    .Where(x =>
                        x.ImportVariantId ==
                        variant.ImportVariantId)
                    .Select(x =>
                        MapFeature(
                            x,
                            importFeatures,
                            productionFeatures,
                            optionRows,
                            valueOptions))
                    .ToList();

            var specificationDtos =
                specificationVariants
                    .Where(x =>
                        x.ImportVariantId ==
                        variant.ImportVariantId)
                    .Select(x =>
                        MapSpecification(
                            x,
                            importSpecifications,
                            productionSpecifications))
                    .ToList();

            var fuelEfficiencyDtos =
                fuelEfficiencyVariants
                    .Where(x =>
                        x.ImportVariantId ==
                        variant.ImportVariantId)
                    .Select(row =>
                    {
                        var fuel =
                            fuelEfficiencies.FirstOrDefault(
                                x =>
                                    x.ImportFuelEfficiencyId ==
                                    row.ImportFuelEfficiencyId);

                        return fuel == null
                            ? null
                            : MapFuelEfficiency(fuel);
                    })
                    .Where(x => x != null)
                    .Cast<StagedFuelEfficiencyDto>()
                    .ToList();

            var warrantyDtos =
                warranties
                    .Where(x =>
                        x.ImportVariantId ==
                        variant.ImportVariantId)
                    .Select(MapWarranty)
                    .ToList();

            return new StagedVariantDetailDto
            {
                ImportVariantId =
                    variant.ImportVariantId,

                ImportVariantParentId =
                    variant.ImportVariantParentId,

                VariantName =
                    variant.VariantName ?? string.Empty,

                VariantType =
                    variant.VariantType ?? string.Empty,

                BaseVariantName =
                    variant.BaseVariantName,

                ExShowroomPrice =
                    variant.ExShowroomPrice,

                KerbWeight =
                    variant.KerbWeight,

                SeatingCapacity =
                    variant.SeatingCapacity,

                PowertrainRef =
                    variant.PowertrainRef,

                TransmissionRef =
                    variant.TransmissionRef,

                DrivetrainRef =
                    variant.DrivetrainRef,

                Powertrain =
                    MapPowertrain(powertrain, motorPerformanceDtos),

                Transmission =
                    MapTransmission(transmission),

                Drivetrain =
                    MapDrivetrain(drivetrain),

                Engine =
                    MapEngine(engine),

                EnginePerformances =
                    enginePerformanceDtos,

                Features =
                    featureDtos,

                Specifications =
                    specificationDtos,

                FuelEfficiencies =
                    fuelEfficiencyDtos,

                Warranties =
                    warrantyDtos
            };
        }

        // ============================================================
        // READ MAPPERS
        // ============================================================

        private static StagedPowertrainDto? MapPowertrain(
            ImportPowertrain? x,
            IReadOnlyCollection<StagedMotorPerformanceDto>? motorPerformances = null)
        {
            if (x == null)
                return null;

            return new StagedPowertrainDto
            {
                ImportPowertrainId =
                    x.ImportPowertrainId,

                PowertrainRef =
                    x.PowertrainRef,

                PowertrainType =
                    x.PowertrainType,

                EngineRef =
                    x.EngineRef,

                BatteryCapacityKWh =
                    x.BatteryCapacityKwh,

                CombinedMaxPower =
                    x.CombinedMaxPower,

                CombinedMaxPowerUnit =
                    x.CombinedMaxPowerUnit,

                CombinedMaxTorque =
                    x.CombinedMaxTorque,

                CombinedMaxTorqueUnit =
                    x.CombinedMaxTorqueUnit,

                CombinedMaxPowerRPM =
                    x.CombinedMaxPowerRpm,

                CombinedMaxPowerRPMMin =
                    x.CombinedMaxPowerRpmmin,

                CombinedMaxPowerRPMMax =
                    x.CombinedMaxPowerRpmmax,

                CombinedMaxTorqueRPM =
                    x.CombinedMaxTorqueRpm,

                CombinedMaxTorqueRPMMin =
                    x.CombinedMaxTorqueRpmmin,

                CombinedMaxTorqueRPMMax =
                    x.CombinedMaxTorqueRpmmax,

                MotorPerformances =
                    motorPerformances?.ToList()
                    ?? []
            };
        }

        private static StagedTransmissionDto? MapTransmission(
            ImportTransmission? x)
        {
            if (x == null)
                return null;

            return new StagedTransmissionDto
            {
                ImportTransmissionId =
                    x.ImportTransmissionId,
                TransmissionRef =
                    x.TransmissionRef,
                TransmissionType =
                    x.TransmissionType,
                NumberOfGears =
                    x.NumberOfGears,
                HasManualOverride =
                    x.HasManualOverride,
                HasPaddleShifters =
                    x.HasPaddleShifters
            };
        }

        private static StagedDrivetrainDto? MapDrivetrain(
            ImportDrivetrain? x)
        {
            if (x == null)
                return null;

            return new StagedDrivetrainDto
            {
                ImportDrivetrainId =
                    x.ImportDrivetrainId,
                DrivetrainRef =
                    x.DrivetrainRef,
                DrivetrainType =
                    x.DrivetrainType,
                DifferentialType =
                    x.DifferentialType
            };
        }

        private static StagedEngineDto? MapEngine(
            ImportEngine? x)
        {
            if (x == null)
                return null;

            return new StagedEngineDto
            {
                ImportEngineId =
                    x.ImportEngineId,
                EngineRef =
                    x.EngineRef,
                EngineName =
                    x.EngineName,
                NumberOfCylinders =
                    x.NumberOfCylinders,
                NumberOfValves =
                    x.NumberOfValves,
                Displacement =
                    x.Displacement,
                IsTurbocharged =
                    x.IsTurbocharged,
                EmissionStandard =
                    x.EmissionStandard,
                Aspiration =
                    x.Aspiration,
                EngineType =
                    x.EngineType
            };
        }

        private static StagedEnginePerformanceDto MapEnginePerformance(
            ImportEnginePerformance x)
        {
            return new StagedEnginePerformanceDto
            {
                ImportEnginePerformanceId =
                    x.ImportEnginePerformanceId,

                ModeName =
                    x.ModeName,

                FuelTypeName =
                    x.FuelTypeName,

                MaxPower =
                    x.MaxPower,

                MaxPowerUnit =
                    x.MaxPowerUnit,

                MaxTorque =
                    x.MaxTorque,

                MaxTorqueUnit =
                    x.MaxTorqueUnit,

                MaxPowerRPM =
                    x.MaxPowerRpm,

                MaxPowerRPMMin =
                    x.MaxPowerRpmmin,

                MaxPowerRPMMax =
                    x.MaxPowerRpmmax,

                MaxTorqueRPM =
                    x.MaxTorqueRpm,

                MaxTorqueRPMMin =
                    x.MaxTorqueRpmmin,

                MaxTorqueRPMMax =
                    x.MaxTorqueRpmmax
            };
        }

        private static StagedMotorPerformanceDto MapMotorPerformance(
            ImportMotorPerformance x)
        {
            return new StagedMotorPerformanceDto
            {
                ImportMotorPerformanceId =
                    x.ImportMotorPerformanceId,

                MotorName =
                    x.MotorName,

                MaxPower =
                    x.MaxPower,

                MaxTorque =
                    x.MaxTorque,

                MaxTorqueUnit =
                    x.MaxTorqueUnit,

                MaxPowerRPM =
                    x.MaxPowerRpm,

                MaxPowerUnit =
                    x.MaxPowerUnit,

                MaxPowerRPMMin =
                    x.MaxPowerRpmmin,

                MaxPowerRPMMax =
                    x.MaxPowerRpmmax,

                MaxTorqueRPM =
                    x.MaxTorqueRpm,

                MaxTorqueRPMMin =
                    x.MaxTorqueRpmmin,

                MaxTorqueRPMMax =
                    x.MaxTorqueRpmmax
            };
        }

        private static StagedFeatureDto MapFeature(
            ImportFeatureVariant row,
            IReadOnlyCollection<ImportFeature> importFeatures,
            IReadOnlyCollection<Feature> productionFeatures,
            IReadOnlyCollection<ImportFeatureValueOption> optionRows,
            IReadOnlyCollection<FeatureValueOption> valueOptions)
        {
            var importFeature =
                importFeatures.FirstOrDefault(
                    x =>
                        x.ImportFeatureId ==
                        row.ImportFeatureId);

            if (importFeature == null)
            {
                throw new InvalidOperationException(
                    $"ImportFeature {row.ImportFeatureId} referenced by " +
                    $"ImportFeatureVariant {row.ImportFeatureVariantId} was not found.");
            }

            Feature? productionFeature = null;

            if (importFeature.FeatureId.HasValue)
            {
                productionFeature =
                    productionFeatures.FirstOrDefault(
                        x =>
                            x.FeatureId ==
                            importFeature.FeatureId.Value);
            }

            var selectedOptionIds =
                optionRows
                    .Where(x =>
                        x.ImportFeatureVariantId ==
                        row.ImportFeatureVariantId)
                    .Select(x =>
                        x.ValueOptionId)
                    .Distinct()
                    .ToHashSet();

            var selectedOptions =
                valueOptions
                    .Where(x =>
                        selectedOptionIds.Contains(
                            x.FeatureValueOptionId))
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x =>
                        new StagedFeatureValueOptionDto
                        {
                            ValueOptionId =
                                x.FeatureValueOptionId,

                            Value =
                                x.Value,

                            DisplayName =
                                x.DisplayName,

                            DisplayOrder =
                                x.DisplayOrder,

                            IsActive =
                                x.IsActive
                        })
                    .ToList();

            return new StagedFeatureDto
            {
                ImportFeatureVariantId =
                    row.ImportFeatureVariantId,

                ImportFeatureId =
                    importFeature.ImportFeatureId,

                FeatureId =
                    importFeature.FeatureId,

                FeatureCode =
                    importFeature.FeatureCode,

                FeatureName =
                    productionFeature?.FeatureName,

                ValueType =
                    productionFeature?.ValueType,

                IsMultiValue =
                    productionFeature?.IsMultiValue ?? false,

                Available =
                    row.Available,

                Value =
                    row.Value,

                ValueOptions =
                    selectedOptions,

                SourceColumn =
                    row.SourceColumn,

                PageNumber =
                    row.PageNumber,

                Evidence =
                    row.Evidence,

                Confidence =
                    row.Confidence
            };
        }

        private static StagedSpecificationDto MapSpecification(
            ImportSpecificationVariant row,
            IReadOnlyCollection<ImportSpecification> importSpecifications,
            IReadOnlyCollection<Specification> productionSpecifications)
        {
            var importSpecification =
                importSpecifications.FirstOrDefault(
                    x =>
                        x.ImportSpecificationId ==
                        row.ImportSpecificationId);

            if (importSpecification == null)
            {
                throw new InvalidOperationException(
                    $"ImportSpecification {row.ImportSpecificationId} referenced by " +
                    $"ImportSpecificationVariant {row.ImportSpecificationVariantId} was not found.");
            }

            Specification? productionSpecification = null;

            if (importSpecification.SpecificationId.HasValue)
            {
                productionSpecification =
                    productionSpecifications.FirstOrDefault(
                        x =>
                            x.SpecificationId ==
                            importSpecification.SpecificationId.Value);
            }

            return new StagedSpecificationDto
            {
                ImportSpecificationVariantId =
                    row.ImportSpecificationVariantId,

                ImportSpecificationId =
                    importSpecification.ImportSpecificationId,

                SpecificationId =
                    importSpecification.SpecificationId,

                SpecificationCode =
                    importSpecification.SpecificationCode,

                SpecificationName =
                    productionSpecification?.SpecificationName,

                DataType =
                    productionSpecification?.DataType,

                Unit =
                    row.Unit ??
                    productionSpecification?.Unit,

                NumericValue =
                    row.NumericValue,

                TextValue =
                    row.TextValue,

                BooleanValue =
                    row.BooleanValue,

                SourceColumn =
                    row.SourceColumn,

                PageNumber =
                    row.PageNumber,

                Evidence =
                    row.Evidence,

                Confidence =
                    row.Confidence
            };
        }

        private static StagedFuelEfficiencyDto MapFuelEfficiency(
            ImportFuelEfficiency x)
        {
            return new StagedFuelEfficiencyDto
            {
                ImportFuelEfficiencyId =
                    x.ImportFuelEfficiencyId,

                FuelEfficiency =
                    x.FuelEfficiency,

                FuelEfficiencyUnit =
                    x.FuelEfficiencyUnit,

                SourceColumn =
                    x.SourceColumn,

                PageNumber =
                    x.PageNumber,

                Evidence =
                    x.Evidence,

                Confidence =
                    x.Confidence
            };
        }

        private static StagedWarrantyDto MapWarranty(
            ImportWarranty x)
        {
            return new StagedWarrantyDto
            {
                ImportWarrantyId =
                    x.ImportWarrantyId,

                WarrantyType =
                    x.WarrantyType,

                DurationYears =
                    x.DurationYears,

                Kilometres =
                    x.Kilometres,

                MaximumDurationYears =
                    x.MaximumDurationYears,

                MaximumKilometres =
                    x.MaximumKilometres,

                SourceId =
                    x.SourceId,

                EvidenceText =
                    x.EvidenceText,

                PageNumber =
                    x.PageNumber,

                Confidence =
                    x.Confidence
            };
        }

        private static StagedModelDimensionsDto MapDimensions(
            ImportModelDimension x)
        {
            return new StagedModelDimensionsDto
            {
                ImportModelDimensionsId =
                    x.ImportModelDimensionId,

                LengthMm =
                    x.LengthMm,

                WidthMm =
                    x.WidthMm,

                HeightMm =
                    x.HeightMm,

                WheelbaseMm =
                    x.WheelbaseMm,

                GroundClearanceMm =
                    x.GroundClearanceMm,

                BootSpaceLitres =
                    x.BootSpaceLitres,

                FuelTankCapacityLitres =
                    x.FuelTankCapacityLitres,

                SourceId =
                    x.SourceId,

                EvidenceText =
                    x.EvidenceText,

                PageNumber =
                    x.PageNumber,

                Confidence =
                    x.Confidence
            };
        }

        // ============================================================
        // MODEL UPDATE
        // ============================================================

        public async Task UpdateModelAsync(
            int importModelId,
            UpdateStagedModelDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(importModelId, nameof(importModelId));

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            try
            {
                await GetStagedBatchForModelAsync(
                    importModelId,
                    cancellationToken);

                var model =
                    await _context.ImportModels
                        .FirstOrDefaultAsync(
                            x =>
                                x.ImportModelId ==
                                importModelId,
                            cancellationToken);

                if (model == null)
                {
                    throw new KeyNotFoundException(
                        $"Staged model {importModelId} was not found.");
                }

                if (dto.BrandName != null)
                {
                    model.BrandName =
                        Required(
                            dto.BrandName,
                            "BrandName");
                }

                if (dto.ModelName != null)
                {
                    model.ModelName =
                        Required(
                            dto.ModelName,
                            "ModelName");
                }

                model.Category =
                    Clean(dto.Category);

                model.BodyType =
                    Clean(dto.BodyType);

                model.ModelImageUrl =
                    Clean(dto.ModelImageUrl);

                if (dto.Dimensions == null)
                {
                    throw new ArgumentException(
                        "Dimensions object cannot be null.",
                        nameof(dto));
                }

                var dimensions =
                    await _context.ImportModelDimensions
                        .FirstOrDefaultAsync(
                            x =>
                                x.ImportModelId ==
                                importModelId,
                            cancellationToken);

                if (dimensions == null)
                {
                    dimensions =
                        new ImportModelDimension
                        {
                            ImportModelId =
                                importModelId
                        };

                    _context.ImportModelDimensions.Add(
                        dimensions);
                }

                dimensions.LengthMm =
                    dto.Dimensions.LengthMm;

                dimensions.WidthMm =
                    dto.Dimensions.WidthMm;

                dimensions.HeightMm =
                    dto.Dimensions.HeightMm;

                dimensions.WheelbaseMm =
                    dto.Dimensions.WheelbaseMm;

                dimensions.GroundClearanceMm =
                    dto.Dimensions.GroundClearanceMm;

                dimensions.BootSpaceLitres =
                    dto.Dimensions.BootSpaceLitres;

                dimensions.FuelTankCapacityLitres =
                    dto.Dimensions.FuelTankCapacityLitres;

                dimensions.SourceId =
                    dto.Dimensions.SourceId;

                dimensions.EvidenceText =
                    Clean(dto.Dimensions.EvidenceText);

                dimensions.PageNumber =
                    dto.Dimensions.PageNumber;

                dimensions.Confidence =
                    dto.Dimensions.Confidence;

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                throw;
            }
        }

        // ============================================================
        // VARIANT UPDATE
        // ============================================================

        public async Task UpdateVariantAsync(
            int importVariantId,
            UpdateStagedVariantDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importVariantId,
                nameof(importVariantId));

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            try
            {
                var variant =
                    await _context.ImportVariants
                        .Include(v =>
                            v.ImportVariantParent)
                        .FirstOrDefaultAsync(
                            v =>
                                v.ImportVariantId ==
                                importVariantId,
                            cancellationToken);

                if (variant == null)
                {
                    throw new KeyNotFoundException(
                        $"Staged variant {importVariantId} was not found.");
                }

                if (variant.ImportVariantParent == null)
                {
                    throw new InvalidOperationException(
                        $"Variant {importVariantId} has no parent.");
                }

                await GetStagedBatchForModelAsync(
                    variant.ImportVariantParent.ImportModelId,
                    cancellationToken);

                if (dto.VariantName != null)
                {
                    var newName =
                        Required(
                            dto.VariantName,
                            "VariantName");

                    var duplicate =
                        await _context.ImportVariants
                            .AnyAsync(
                                x =>
                                    x.ImportVariantId !=
                                        importVariantId &&
                                    x.ImportVariantParentId ==
                                        variant.ImportVariantParentId &&
                                    x.VariantName.ToLower() ==
                                        newName.ToLower(),
                                cancellationToken);

                    if (duplicate)
                    {
                        throw new InvalidOperationException(
                            $"Variant '{newName}' already exists under this parent.");
                    }

                    variant.VariantName =
                        newName;
                }

                variant.BaseVariantName =
                    Clean(dto.BaseVariantName);

                variant.ExShowroomPrice =
                    dto.ExShowroomPrice;

                variant.KerbWeight =
                    dto.KerbWeight;

                variant.SeatingCapacity =
                    dto.SeatingCapacity;

                await ResolveVariantPowertrainAsync(
                    variant,
                    dto.PowertrainRef,
                    cancellationToken);

                await ResolveVariantTransmissionAsync(
                    variant,
                    dto.TransmissionRef,
                    cancellationToken);

                await ResolveVariantDrivetrainAsync(
                    variant,
                    dto.DrivetrainRef,
                    cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                throw;
            }
        }

        // ============================================================
        // VARIANT REF RESOLUTION
        // ============================================================

        private async Task ResolveVariantPowertrainAsync(
            ImportVariant variant,
            string? powertrainRef,
            CancellationToken cancellationToken)
        {
            powertrainRef =
                Clean(powertrainRef);

            if (powertrainRef == null)
            {
                variant.PowertrainRef = null;
                variant.ImportPowertrainId = null;
                return;
            }

            if (variant.ImportVariantParent == null)
            {
                throw new InvalidOperationException(
                    $"Variant {variant.ImportVariantId} has no parent.");
            }

            var powertrain =
                await _context.ImportPowertrains
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportModelId ==
                                variant.ImportVariantParent.ImportModelId &&
                            x.PowertrainRef ==
                                powertrainRef,
                        cancellationToken);

            if (powertrain == null)
            {
                throw new InvalidOperationException(
                    $"PowertrainRef '{powertrainRef}' does not exist for this staged model.");
            }

            variant.PowertrainRef =
                powertrain.PowertrainRef;

            variant.ImportPowertrainId =
                powertrain.ImportPowertrainId;
        }

        private async Task ResolveVariantTransmissionAsync(
            ImportVariant variant,
            string? transmissionRef,
            CancellationToken cancellationToken)
        {
            transmissionRef =
                Clean(transmissionRef);

            if (transmissionRef == null)
            {
                variant.TransmissionRef = null;
                variant.ImportTransmissionId = null;
                return;
            }

            if (variant.ImportVariantParent == null)
            {
                throw new InvalidOperationException(
                    $"Variant {variant.ImportVariantId} has no parent.");
            }

            var transmission =
                await _context.ImportTransmissions
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportModelId ==
                                variant.ImportVariantParent.ImportModelId &&
                            x.TransmissionRef ==
                                transmissionRef,
                        cancellationToken);

            if (transmission == null)
            {
                throw new InvalidOperationException(
                    $"TransmissionRef '{transmissionRef}' does not exist for this staged model.");
            }

            variant.TransmissionRef =
                transmission.TransmissionRef;

            variant.ImportTransmissionId =
                transmission.ImportTransmissionId;
        }

        private async Task ResolveVariantDrivetrainAsync(
            ImportVariant variant,
            string? drivetrainRef,
            CancellationToken cancellationToken)
        {
            drivetrainRef =
                Clean(drivetrainRef);

            if (drivetrainRef == null)
            {
                variant.DrivetrainRef = null;
                variant.ImportDrivetrainId = null;
                return;
            }

            if (variant.ImportVariantParent == null)
            {
                throw new InvalidOperationException(
                    $"Variant {variant.ImportVariantId} has no parent.");
            }

            var drivetrain =
                await _context.ImportDrivetrains
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportModelId ==
                                variant.ImportVariantParent.ImportModelId &&
                            x.DrivetrainRef ==
                                drivetrainRef,
                        cancellationToken);

            if (drivetrain == null)
            {
                throw new InvalidOperationException(
                    $"DrivetrainRef '{drivetrainRef}' does not exist for this staged model.");
            }

            variant.DrivetrainRef =
                drivetrain.DrivetrainRef;

            variant.ImportDrivetrainId =
                drivetrain.ImportDrivetrainId;
        }

        // ============================================================
        // POWERTRAIN UPDATE
        // ============================================================

        public async Task UpdatePowertrainAsync(
            int importPowertrainId,
            UpdateStagedPowertrainDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var modelId =
                await GetModelIdForPowertrainAsync(
                    importPowertrainId,
                    cancellationToken);

            await GetStagedBatchForModelAsync(
                modelId,
                cancellationToken);

            var powertrain =
                await _context.ImportPowertrains
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportPowertrainId ==
                            importPowertrainId,
                        cancellationToken);

            if (powertrain == null)
            {
                throw new KeyNotFoundException(
                    $"Staged powertrain {importPowertrainId} was not found.");
            }

            var type =
                Clean(dto.PowertrainType);

            if (type != null &&
                !new[] { "ICE", "EV", "Hybrid" }
                    .Contains(
                        type,
                        StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "PowertrainType must be ICE, EV or Hybrid.");
            }

            powertrain.PowertrainType =
                type;

            powertrain.EngineRef =
                Clean(dto.EngineRef);

            powertrain.BatteryCapacityKwh =
                dto.BatteryCapacityKWh;

            powertrain.CombinedMaxPower =
                dto.CombinedMaxPower;

            powertrain.CombinedMaxTorque =
                dto.CombinedMaxTorque;

            powertrain.CombinedMaxPowerRpm =
                dto.CombinedMaxPowerRPM;

            powertrain.CombinedMaxPowerRpmmin =
                dto.CombinedMaxPowerRPMMin;

            powertrain.CombinedMaxPowerRpmmax =
                dto.CombinedMaxPowerRPMMax;

            powertrain.CombinedMaxTorqueRpm =
                dto.CombinedMaxTorqueRPM;

            powertrain.CombinedMaxTorqueRpmmin =
                dto.CombinedMaxTorqueRPMMin;

            powertrain.CombinedMaxTorqueRpmmax =
                dto.CombinedMaxTorqueRPMMax;

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // TRANSMISSION UPDATE
        // ============================================================

        public async Task UpdateTransmissionAsync(
            int importTransmissionId,
            UpdateStagedTransmissionDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var modelId =
                await GetModelIdForTransmissionAsync(
                    importTransmissionId,
                    cancellationToken);

            await GetStagedBatchForModelAsync(
                modelId,
                cancellationToken);

            var transmission =
                await _context.ImportTransmissions
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportTransmissionId ==
                            importTransmissionId,
                        cancellationToken);

            if (transmission == null)
            {
                throw new KeyNotFoundException(
                    $"Staged transmission {importTransmissionId} was not found.");
            }

            transmission.TransmissionType =
                Clean(dto.TransmissionType);

            transmission.NumberOfGears =
                dto.NumberOfGears;

            transmission.HasManualOverride =
                dto.HasManualOverride;

            transmission.HasPaddleShifters =
                dto.HasPaddleShifters;

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // DRIVETRAIN UPDATE
        // ============================================================

        public async Task UpdateDrivetrainAsync(
            int importDrivetrainId,
            UpdateStagedDrivetrainDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var modelId =
                await GetModelIdForDrivetrainAsync(
                    importDrivetrainId,
                    cancellationToken);

            await GetStagedBatchForModelAsync(
                modelId,
                cancellationToken);

            var drivetrain =
                await _context.ImportDrivetrains
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportDrivetrainId ==
                            importDrivetrainId,
                        cancellationToken);

            if (drivetrain == null)
            {
                throw new KeyNotFoundException(
                    $"Staged drivetrain {importDrivetrainId} was not found.");
            }

            drivetrain.DrivetrainType =
                Clean(dto.DrivetrainType);

            drivetrain.DifferentialType =
                Clean(dto.DifferentialType);

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // ENGINE UPDATE
        // ============================================================

        public async Task UpdateEngineAsync(
            int importEngineId,
            UpdateStagedEngineDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importEngineId,
                nameof(importEngineId));

            var engine =
                await _context.ImportEngines
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportEngineId ==
                            importEngineId,
                        cancellationToken);

            if (engine == null)
            {
                throw new KeyNotFoundException(
                    $"Staged engine {importEngineId} was not found.");
            }

            var modelId =
                await _context.ImportPowertrains
                    .Where(x =>
                        x.ImportPowertrainId ==
                        engine.ImportPowertrainId)
                    .Select(x =>
                        (int?)x.ImportModelId)
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (!modelId.HasValue)
            {
                throw new InvalidOperationException(
                    $"Powertrain for staged engine {importEngineId} was not found.");
            }

            await GetStagedBatchForModelAsync(
                modelId.Value,
                cancellationToken);

            if (dto.NumberOfCylinders is < 0 or > 255)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(dto.NumberOfCylinders),
                    "NumberOfCylinders must be between 0 and 255.");
            }

            if (dto.NumberOfValves is < 0 or > 255)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(dto.NumberOfValves),
                    "NumberOfValves must be between 0 and 255.");
            }

            engine.EngineName =
                Clean(dto.EngineName);

            engine.NumberOfCylinders =
                dto.NumberOfCylinders.HasValue
                    ? (byte?)dto.NumberOfCylinders.Value
                    : null;

            engine.NumberOfValves =
                dto.NumberOfValves.HasValue
                    ? (byte?)dto.NumberOfValves.Value
                    : null;

            engine.Displacement =
                dto.Displacement;

            engine.IsTurbocharged =
                dto.IsTurbocharged ?? false;

            engine.EmissionStandard =
                Clean(dto.EmissionStandard);

            engine.Aspiration =
                Clean(dto.Aspiration);

            engine.EngineType =
                Clean(dto.EngineType);

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // ENGINE PERFORMANCE UPDATE
        // ============================================================

        public async Task UpdateEnginePerformanceAsync(
            int importEnginePerformanceId,
            UpdateStagedEnginePerformanceDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importEnginePerformanceId,
                nameof(importEnginePerformanceId));

            var performance =
                await _context.ImportEnginePerformances
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportEnginePerformanceId ==
                            importEnginePerformanceId,
                        cancellationToken);

            if (performance == null)
            {
                throw new KeyNotFoundException(
                    $"Staged engine performance {importEnginePerformanceId} was not found.");
            }

            var modelId =
                await _context.ImportEngines
                    .Where(x =>
                        x.ImportEngineId ==
                        performance.ImportEngineId)
                    .Join(
                        _context.ImportPowertrains,
                        engine => engine.ImportPowertrainId,
                        powertrain => powertrain.ImportPowertrainId,
                        (engine, powertrain) =>
                            (int?)powertrain.ImportModelId)
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (!modelId.HasValue)
            {
                throw new InvalidOperationException(
                    $"Unable to resolve model for engine performance {importEnginePerformanceId}.");
            }

            await GetStagedBatchForModelAsync(
                modelId.Value,
                cancellationToken);

            performance.ModeName =
                Clean(dto.ModeName);

            performance.FuelTypeName =
                Clean(dto.FuelTypeName);

            // FuelTypeId is intentionally never used in staging.
            performance.FuelTypeId = null;

            performance.MaxPower =
                dto.MaxPower;

            performance.MaxPowerUnit =
                Clean(dto.MaxPowerUnit);

            performance.MaxTorque =
                dto.MaxTorque;

            performance.MaxTorqueUnit =
                Clean(dto.MaxTorqueUnit);

            performance.MaxPowerRpm =
                dto.MaxPowerRPM;

            performance.MaxPowerRpmmin =
                dto.MaxPowerRPMMin;

            performance.MaxPowerRpmmax =
                dto.MaxPowerRPMMax;

            performance.MaxTorqueRpm =
                dto.MaxTorqueRPM;

            performance.MaxTorqueRpmmin =
                dto.MaxTorqueRPMMin;

            performance.MaxTorqueRpmmax =
                dto.MaxTorqueRPMMax;

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // MOTOR PERFORMANCE UPDATE
        // ============================================================

        public async Task UpdateMotorPerformanceAsync(
            int importMotorPerformanceId,
            UpdateStagedMotorPerformanceDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importMotorPerformanceId,
                nameof(importMotorPerformanceId));

            var performance =
                await _context.ImportMotorPerformances
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportMotorPerformanceId ==
                            importMotorPerformanceId,
                        cancellationToken);

            if (performance == null)
            {
                throw new KeyNotFoundException(
                    $"Staged motor performance {importMotorPerformanceId} was not found.");
            }

            var modelId =
                await GetModelIdForPowertrainAsync(
                    performance.ImportPowertrainId,
                    cancellationToken);

            await GetStagedBatchForModelAsync(
                modelId,
                cancellationToken);

            performance.MotorName =
                Clean(dto.MotorName);

            performance.MaxPower =
                dto.MaxPower;

            performance.MaxTorque =
                dto.MaxTorque;

            performance.MaxPowerRpm =
                dto.MaxPowerRPM;

            performance.MaxPowerRpmmin =
                dto.MaxPowerRPMMin;

            performance.MaxPowerRpmmax =
                dto.MaxPowerRPMMax;

            performance.MaxTorqueRpm =
                dto.MaxTorqueRPM;

            performance.MaxTorqueRpmmin =
                dto.MaxTorqueRPMMin;

            performance.MaxTorqueRpmmax =
                dto.MaxTorqueRPMMax;

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // FEATURE VARIANT UPDATE
        // ============================================================

        public async Task UpdateFeatureVariantAsync(
            int importFeatureVariantId,
            UpdateStagedFeatureDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importFeatureVariantId,
                nameof(importFeatureVariantId));

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            try
            {
                var row =
                    await _context.ImportFeatureVariants
                        .FirstOrDefaultAsync(
                            x =>
                                x.ImportFeatureVariantId ==
                                importFeatureVariantId,
                            cancellationToken);

                if (row == null)
                {
                    throw new KeyNotFoundException(
                        $"Staged feature variant {importFeatureVariantId} was not found.");
                }

                var modelId =
                    await GetModelIdForVariantAsync(
                        row.ImportVariantId,
                        cancellationToken);

                await GetStagedBatchForModelAsync(
                    modelId,
                    cancellationToken);

                var importFeature =
                    await _context.ImportFeatures
                        .FirstOrDefaultAsync(
                            x =>
                                x.ImportFeatureId ==
                                row.ImportFeatureId,
                            cancellationToken);

                if (importFeature == null)
                {
                    throw new InvalidOperationException(
                        $"ImportFeature {row.ImportFeatureId} was not found.");
                }

                row.Available =
                    dto.Available;

                row.Value =
                    dto.Value;

                row.SourceColumn =
                    Clean(dto.SourceColumn);

                row.PageNumber =
                    dto.PageNumber;

                row.Evidence =
                    dto.Evidence;

                row.Confidence =
                    dto.Confidence;

                var requestedOptionIds =
                    (dto.ValueOptionIds ?? [])
                        .Distinct()
                        .ToList();

                if (requestedOptionIds.Any(x => x <= 0))
                {
                    throw new ArgumentException(
                        "All feature value option IDs must be greater than zero.");
                }

                // A production FeatureId is required for production
                // FeatureValueOption validation.
                if (requestedOptionIds.Count > 0)
                {
                    if (!importFeature.FeatureId.HasValue)
                    {
                        throw new InvalidOperationException(
                            $"Feature '{importFeature.FeatureCode}' has no Production FeatureId. " +
                            "Value options cannot be assigned until the feature is linked to a production feature.");
                    }

                    var validOptionIds =
                        await _context.FeatureValueOptions
                            .Where(x =>
                                x.FeatureId ==
                                    importFeature.FeatureId.Value &&
                                requestedOptionIds.Contains(
                                    x.FeatureValueOptionId))
                            .Select(x =>
                                x.FeatureValueOptionId)
                            .ToListAsync(
                                cancellationToken);

                    var invalidOptionIds =
                        requestedOptionIds
                            .Except(validOptionIds)
                            .ToList();

                    if (invalidOptionIds.Count > 0)
                    {
                        throw new InvalidOperationException(
                            $"Value option(s) {string.Join(", ", invalidOptionIds)} " +
                            $"do not belong to FeatureId {importFeature.FeatureId.Value}.");
                    }
                }

                var existingRows =
                    await _context.ImportFeatureValueOptions
                        .Where(x =>
                            x.ImportFeatureVariantId ==
                            importFeatureVariantId)
                        .ToListAsync(
                            cancellationToken);

                if (existingRows.Count > 0)
                {
                    _context.ImportFeatureValueOptions.RemoveRange(
                        existingRows);
                }

                foreach (var optionId in requestedOptionIds)
                {
                    _context.ImportFeatureValueOptions.Add(
                        new ImportFeatureValueOption
                        {
                            ImportFeatureVariantId =
                                importFeatureVariantId,

                            ValueOptionId =
                                optionId
                        });
                }

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                throw;
            }
        }

        // ============================================================
        // SPECIFICATION VARIANT UPDATE
        // ============================================================

        public async Task UpdateSpecificationVariantAsync(
            int importSpecificationVariantId,
            UpdateStagedSpecificationDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importSpecificationVariantId,
                nameof(importSpecificationVariantId));

            var row =
                await _context.ImportSpecificationVariants
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportSpecificationVariantId ==
                            importSpecificationVariantId,
                        cancellationToken);

            if (row == null)
            {
                throw new KeyNotFoundException(
                    $"Staged specification variant {importSpecificationVariantId} was not found.");
            }

            var modelId =
                await GetModelIdForVariantAsync(
                    row.ImportVariantId,
                    cancellationToken);

            await GetStagedBatchForModelAsync(
                modelId,
                cancellationToken);

            var specification =
                await _context.ImportSpecifications
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportSpecificationId ==
                            row.ImportSpecificationId,
                        cancellationToken);

            if (specification == null)
            {
                throw new InvalidOperationException(
                    $"ImportSpecification {row.ImportSpecificationId} was not found.");
            }

            row.NumericValue =
                dto.NumericValue;

            row.TextValue =
                dto.TextValue;

            row.BooleanValue =
                dto.BooleanValue;

            row.Unit =
                Clean(dto.Unit);

            row.SourceColumn =
                Clean(dto.SourceColumn);

            row.PageNumber =
                dto.PageNumber;

            row.Evidence =
                dto.Evidence;

            row.Confidence =
                dto.Confidence;

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // FUEL EFFICIENCY UPDATE
        // ============================================================

        public async Task UpdateFuelEfficiencyAsync(
            int importFuelEfficiencyId,
            UpdateStagedFuelEfficiencyDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importFuelEfficiencyId,
                nameof(importFuelEfficiencyId));

            var entity =
                await _context.ImportFuelEfficiencies
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportFuelEfficiencyId ==
                            importFuelEfficiencyId,
                        cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Staged fuel efficiency {importFuelEfficiencyId} was not found.");
            }

            var variantIds =
                await _context.ImportFuelEfficiencyVariants
                    .Where(x =>
                        x.ImportFuelEfficiencyId ==
                        importFuelEfficiencyId)
                    .Select(x =>
                        x.ImportVariantId)
                    .ToListAsync(
                        cancellationToken);

            if (variantIds.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Fuel efficiency {importFuelEfficiencyId} is not attached to any staged variant.");
            }

            var modelId =
                await _context.ImportVariants
                    .Where(x =>
                        variantIds.Contains(
                            x.ImportVariantId))
                    .Select(x =>
                        (int?)x.ImportVariantParent.ImportModelId)
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (!modelId.HasValue)
            {
                throw new InvalidOperationException(
                    $"Unable to resolve model for fuel efficiency {importFuelEfficiencyId}.");
            }

            await GetStagedBatchForModelAsync(
                modelId.Value,
                cancellationToken);

            entity.FuelEfficiency =
                dto.FuelEfficiency;

            entity.FuelEfficiencyUnit =
                Clean(dto.FuelEfficiencyUnit);

            entity.SourceColumn =
                Clean(dto.SourceColumn);

            entity.PageNumber =
                dto.PageNumber;

            entity.Evidence =
                dto.Evidence;

            entity.Confidence =
                dto.Confidence;

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // WARRANTY UPDATE
        // ============================================================

        public async Task UpdateWarrantyAsync(
            int importWarrantyId,
            UpdateStagedWarrantyDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importWarrantyId,
                nameof(importWarrantyId));

            var entity =
                await _context.ImportWarranties
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportWarrantyId ==
                            importWarrantyId,
                        cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Staged warranty {importWarrantyId} was not found.");
            }

            await GetStagedBatchForModelAsync(
                entity.ImportModelId,
                cancellationToken);

            if (dto.WarrantyType != null)
            {
                entity.WarrantyType =
                    Required(
                        dto.WarrantyType,
                        "WarrantyType");
            }

            entity.DurationYears =
                dto.DurationYears;

            entity.Kilometres =
                dto.Kilometres;

            entity.MaximumDurationYears =
                dto.MaximumDurationYears;

            entity.MaximumKilometres =
                dto.MaximumKilometres;

            entity.SourceId =
                dto.SourceId;

            entity.EvidenceText =
                Clean(dto.EvidenceText);

            entity.PageNumber =
                dto.PageNumber;

            entity.Confidence =
                dto.Confidence;

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // CREATE POWERTRAIN
        // ============================================================

        public async Task<int> CreatePowertrainAsync(
            CreateStagedPowertrainDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                dto.ImportModelId,
                nameof(dto.ImportModelId));

            var powertrainRef =
                Required(
                    dto.PowertrainRef,
                    "PowertrainRef");

            var powertrainType =
                Required(
                    dto.PowertrainType,
                    "PowertrainType");

            if (!new[]
                {
            "ICE",
            "EV",
            "Hybrid"
        }
                .Contains(
                    powertrainType,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "PowertrainType must be ICE, EV or Hybrid.");
            }

            await GetStagedBatchForModelAsync(
                dto.ImportModelId,
                cancellationToken);

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            try
            {
                // ------------------------------------------------------------
                // POWERTRAIN
                // ------------------------------------------------------------

                var powertrainExists =
                    await _context.ImportPowertrains
                        .AnyAsync(
                            x =>
                                x.ImportModelId ==
                                    dto.ImportModelId &&
                                x.PowertrainRef ==
                                    powertrainRef,
                            cancellationToken);

                if (powertrainExists)
                {
                    throw new InvalidOperationException(
                        $"PowertrainRef '{powertrainRef}' already exists for this model.");
                }

                var powertrain =
                    new ImportPowertrain
                    {
                        ImportModelId =
                            dto.ImportModelId,

                        PowertrainRef =
                            powertrainRef,

                        PowertrainType =
                            powertrainType,

                        EngineRef =
                            Clean(dto.EngineRef),

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

                        ProductionPowertrainId =
                            null
                    };

                _context.ImportPowertrains.Add(powertrain);

                await _context.SaveChangesAsync(
                    cancellationToken);


                // ------------------------------------------------------------
                // ENGINE
                // ------------------------------------------------------------

                ImportEngine? engine = null;

                var engineMode =
                    Clean(dto.EngineMode)?
                        .ToLowerInvariant()
                        ?? "none";

                if (engineMode == "existing")
                {
                    if (!dto.ExistingEngineId.HasValue)
                    {
                        throw new ArgumentException(
                            "ExistingEngineId is required when EngineMode is 'existing'.");
                    }

                    var sourceEngine =
                        await _context.ImportEngines
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                x =>
                                    x.ImportEngineId ==
                                    dto.ExistingEngineId.Value,
                                cancellationToken);

                    if (sourceEngine == null)
                    {
                        throw new KeyNotFoundException(
                            $"Existing staged engine {dto.ExistingEngineId.Value} was not found.");
                    }

                    // Clone the existing engine under the new powertrain.
                    engine =
                        new ImportEngine
                        {
                            ImportPowertrainId =
                                powertrain.ImportPowertrainId,

                            EngineRef =
                                sourceEngine.EngineRef,

                            EngineName =
                                sourceEngine.EngineName,

                            NumberOfCylinders =
                                sourceEngine.NumberOfCylinders,

                            NumberOfValves =
                                sourceEngine.NumberOfValves,

                            Displacement =
                                sourceEngine.Displacement,

                            IsTurbocharged =
                                sourceEngine.IsTurbocharged,

                            EmissionStandard =
                                sourceEngine.EmissionStandard,

                            Aspiration =
                                sourceEngine.Aspiration,

                            EngineType =
                                sourceEngine.EngineType,

                            ProductionEngineId =
                                sourceEngine.ProductionEngineId
                        };

                    _context.ImportEngines.Add(engine);

                    await _context.SaveChangesAsync(
                        cancellationToken);

                    powertrain.EngineRef =
                        engine.EngineRef;

                    await _context.SaveChangesAsync(
                        cancellationToken);
                }
                else if (engineMode == "new")
                {
                    if (dto.NewEngine == null)
                    {
                        throw new ArgumentException(
                            "NewEngine is required when EngineMode is 'new'.");
                    }

                    var engineRef =
                        Required(
                            dto.NewEngine.EngineRef,
                            "NewEngine.EngineRef");

                    var engineName =
                        Required(
                            dto.NewEngine.EngineName,
                            "NewEngine.EngineName");

                    engine =
                        new ImportEngine
                        {
                            ImportPowertrainId =
                                powertrain.ImportPowertrainId,

                            EngineRef =
                                engineRef,

                            EngineName =
                                engineName,

                            NumberOfCylinders =
                                dto.NewEngine.NumberOfCylinders,

                            NumberOfValves =
                                dto.NewEngine.NumberOfValves,

                            Displacement =
                                dto.NewEngine.Displacement,

                            IsTurbocharged =
                                dto.NewEngine.IsTurbocharged,

                            EmissionStandard =
                                Clean(dto.NewEngine.EmissionStandard),

                            Aspiration =
                                Clean(dto.NewEngine.Aspiration),

                            EngineType =
                                Clean(dto.NewEngine.EngineType),

                            ProductionEngineId =
                                null
                        };

                    _context.ImportEngines.Add(engine);

                    await _context.SaveChangesAsync(
                        cancellationToken);

                    powertrain.EngineRef =
                        engine.EngineRef;

                    await _context.SaveChangesAsync(
                        cancellationToken);
                }
                else if (engineMode != "none")
                {
                    throw new ArgumentException(
                        "EngineMode must be 'none', 'existing' or 'new'.");
                }


                // ------------------------------------------------------------
                // ENGINE PERFORMANCE
                // ------------------------------------------------------------

                var performanceMode =
                    Clean(dto.EnginePerformanceMode)?
                        .ToLowerInvariant()
                        ?? "none";

                if (performanceMode != "none")
                {
                    if (engine == null)
                    {
                        throw new InvalidOperationException(
                            "An engine must exist before creating engine performance.");
                    }

                    if (performanceMode == "existing")
                    {
                        if (!dto.ExistingEnginePerformanceId.HasValue)
                        {
                            throw new ArgumentException(
                                "ExistingEnginePerformanceId is required.");
                        }

                        var source =
                            await _context.ImportEnginePerformances
                                .AsNoTracking()
                                .FirstOrDefaultAsync(
                                    x =>
                                        x.ImportEnginePerformanceId ==
                                        dto.ExistingEnginePerformanceId.Value,
                                    cancellationToken);

                        if (source == null)
                        {
                            throw new KeyNotFoundException(
                                $"Existing engine performance {dto.ExistingEnginePerformanceId.Value} was not found.");
                        }

                        var performance =
                            new ImportEnginePerformance
                            {
                                ImportEngineId =
                                    engine.ImportEngineId,

                                ModeName =
                                    source.ModeName,

                                FuelTypeId =
                                    source.FuelTypeId,

                                MaxPower =
                                    source.MaxPower,

                                MaxTorque =
                                    source.MaxTorque,

                                MaxPowerRpm =
                                    source.MaxPowerRpm,

                                MaxPowerRpmmin =
                                    source.MaxPowerRpmmin,

                                MaxPowerRpmmax =
                                    source.MaxPowerRpmmax,

                                MaxTorqueRpm =
                                    source.MaxTorqueRpm,

                                MaxTorqueRpmmin =
                                    source.MaxTorqueRpmmin,

                                MaxTorqueRpmmax =
                                    source.MaxTorqueRpmmax,

                                ProductionEnginePerformanceId =
                                    source.ProductionEnginePerformanceId
                            };

                        _context.ImportEnginePerformances.Add(
                            performance);
                    }
                    else if (performanceMode == "new")
                    {
                        if (dto.NewEnginePerformance == null)
                        {
                            throw new ArgumentException(
                                "NewEnginePerformance is required.");
                        }

                        var source =
                            dto.NewEnginePerformance;

                        var performance =
                            new ImportEnginePerformance
                            {
                                ImportEngineId =
                                    engine.ImportEngineId,

                                ModeName =
                                    Required(
                                        source.ModeName,
                                        "NewEnginePerformance.ModeName"),

                                FuelTypeId =
                                    source.FuelTypeId,

                                MaxPower =
                                    source.MaxPower,

                                MaxTorque =
                                    source.MaxTorque,

                                MaxPowerRpm =
                                    source.MaxPowerRPM,

                                MaxPowerRpmmin =
                                    source.MaxPowerRPMMin,

                                MaxPowerRpmmax =
                                    source.MaxPowerRPMMax,

                                MaxTorqueRpm =
                                    source.MaxTorqueRPM,

                                MaxTorqueRpmmin =
                                    source.MaxTorqueRPMMin,

                                MaxTorqueRpmmax =
                                    source.MaxTorqueRPMMax,

                                ProductionEnginePerformanceId =
                                    null
                            };

                        _context.ImportEnginePerformances.Add(
                            performance);
                    }
                    else
                    {
                        throw new ArgumentException(
                            "EnginePerformanceMode must be 'none', 'existing' or 'new'.");
                    }

                    await _context.SaveChangesAsync(
                        cancellationToken);
                }


                // ------------------------------------------------------------
                // MOTOR PERFORMANCE
                // ------------------------------------------------------------

                foreach (
                    var existingMotorId
                    in dto.ExistingMotorPerformanceIds
                        ?? [])
                {
                    var source =
                        await _context.ImportMotorPerformances
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                x =>
                                    x.ImportMotorPerformanceId ==
                                    existingMotorId,
                                cancellationToken);

                    if (source == null)
                    {
                        throw new KeyNotFoundException(
                            $"Existing motor performance {existingMotorId} was not found.");
                    }

                    var motor =
                        new ImportMotorPerformance
                        {
                            ImportPowertrainId =
                                powertrain.ImportPowertrainId,

                            MotorName =
                                source.MotorName,

                            MaxPower =
                                source.MaxPower,

                            MaxTorque =
                                source.MaxTorque,

                            MaxPowerRpm =
                                source.MaxPowerRpm,

                            MaxPowerRpmmin =
                                source.MaxPowerRpmmin,

                            MaxPowerRpmmax =
                                source.MaxPowerRpmmax,

                            MaxTorqueRpm =
                                source.MaxTorqueRpm,

                            MaxTorqueRpmmin =
                                source.MaxTorqueRpmmin,

                            MaxTorqueRpmmax =
                                source.MaxTorqueRpmmax,

                            ProductionMotorPerformanceId =
                                source.ProductionMotorPerformanceId
                        };

                    _context.ImportMotorPerformances.Add(
                        motor);
                }


                foreach (
                    var source
                    in dto.NewMotorPerformances
                        ?? [])
                {
                    var motor =
                        new ImportMotorPerformance
                        {
                            ImportPowertrainId =
                                powertrain.ImportPowertrainId,

                            MotorName =
                                Clean(source.MotorName),

                            MaxPower =
                                source.MaxPower,

                            MaxTorque =
                                source.MaxTorque,

                            MaxPowerRpm =
                                source.MaxPowerRPM,

                            MaxPowerRpmmin =
                                source.MaxPowerRPMMin,

                            MaxPowerRpmmax =
                                source.MaxPowerRPMMax,

                            MaxTorqueRpm =
                                source.MaxTorqueRPM,

                            MaxTorqueRpmmin =
                                source.MaxTorqueRPMMin,

                            MaxTorqueRpmmax =
                                source.MaxTorqueRPMMax,

                            ProductionMotorPerformanceId =
                                null
                        };

                    _context.ImportMotorPerformances.Add(
                        motor);
                }

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                return powertrain.ImportPowertrainId;
            }
            catch
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                throw;
            }
        }

        public async Task<int> CreateEngineAsync(
    int importPowertrainId,
    CreateStagedEngineDto dto,
    CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importPowertrainId,
                nameof(importPowertrainId));

            var powertrain =
                await _context.ImportPowertrains
                    .FirstOrDefaultAsync(
                        x => x.ImportPowertrainId == importPowertrainId,
                        cancellationToken);

            if (powertrain == null)
            {
                throw new KeyNotFoundException(
                    $"Staged powertrain {importPowertrainId} was not found.");
            }

            await GetStagedBatchForModelAsync(
                powertrain.ImportModelId,
                cancellationToken);

            var engineRef =
                Required(
                    dto.EngineRef,
                    "EngineRef");

            var engineName =
                Required(
                    dto.EngineName,
                    "EngineName");

            var exists =
                await _context.ImportEngines
                    .AnyAsync(
                        x =>
                            x.ImportPowertrainId == importPowertrainId &&
                            x.EngineRef == engineRef,
                        cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"EngineRef '{engineRef}' already exists for this powertrain.");
            }

            var engine =
                new ImportEngine
                {
                    ImportPowertrainId =
                        importPowertrainId,

                    EngineRef =
                        engineRef,

                    EngineName =
                        engineName,

                    NumberOfCylinders =
                        dto.NumberOfCylinders,

                    NumberOfValves =
                        dto.NumberOfValves,

                    Displacement =
                        dto.Displacement,

                    IsTurbocharged =
                        dto.IsTurbocharged,

                    EmissionStandard =
                        Clean(dto.EmissionStandard),

                    Aspiration =
                        Clean(dto.Aspiration),

                    EngineType =
                        Clean(dto.EngineType),

                    ProductionEngineId =
                        null
                };

            _context.ImportEngines.Add(engine);

            await _context.SaveChangesAsync(
                cancellationToken);

            return engine.ImportEngineId;
        }

        public async Task<int> CreateEnginePerformanceAsync(
    int importEngineId,
    CreateStagedEnginePerformanceDto dto,
    CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importEngineId,
                nameof(importEngineId));

            var engine =
                await _context.ImportEngines
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportEngineId ==
                            importEngineId,
                        cancellationToken);

            if (engine == null)
            {
                throw new KeyNotFoundException(
                    $"Staged engine {importEngineId} was not found.");
            }

            var powertrainModelId =
                await _context.ImportPowertrains
                    .Where(x =>
                        x.ImportPowertrainId ==
                        engine.ImportPowertrainId)
                    .Select(x =>
                        (int?)x.ImportModelId)
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (!powertrainModelId.HasValue)
            {
                throw new InvalidOperationException(
                    $"Unable to resolve model for engine {importEngineId}.");
            }

            await GetStagedBatchForModelAsync(
                powertrainModelId.Value,
                cancellationToken);

            var modeName =
                Required(
                    dto.ModeName,
                    "ModeName");

            var performance =
                new ImportEnginePerformance
                {
                    ImportEngineId =
                        importEngineId,

                    ModeName =
                        modeName,

                    FuelTypeId =
                        dto.FuelTypeId,

                    MaxPower =
                        dto.MaxPower,

                    MaxTorque =
                        dto.MaxTorque,

                    MaxPowerRpm =
                        dto.MaxPowerRPM,

                    MaxPowerRpmmin =
                        dto.MaxPowerRPMMin,

                    MaxPowerRpmmax =
                        dto.MaxPowerRPMMax,

                    MaxTorqueRpm =
                        dto.MaxTorqueRPM,

                    MaxTorqueRpmmin =
                        dto.MaxTorqueRPMMin,

                    MaxTorqueRpmmax =
                        dto.MaxTorqueRPMMax,

                    ProductionEnginePerformanceId =
                        null
                };

            _context.ImportEnginePerformances.Add(
                performance);

            await _context.SaveChangesAsync(
                cancellationToken);

            return performance.ImportEnginePerformanceId;
        }


        public async Task<int> CreateMotorPerformanceAsync(
    int importPowertrainId,
    CreateStagedMotorPerformanceDto dto,
    CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importPowertrainId,
                nameof(importPowertrainId));

            var powertrain =
                await _context.ImportPowertrains
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportPowertrainId ==
                            importPowertrainId,
                        cancellationToken);

            if (powertrain == null)
            {
                throw new KeyNotFoundException(
                    $"Staged powertrain {importPowertrainId} was not found.");
            }

            await GetStagedBatchForModelAsync(
                powertrain.ImportModelId,
                cancellationToken);

            var performance =
                new ImportMotorPerformance
                {
                    ImportPowertrainId =
                        importPowertrainId,

                    MotorName =
                        Clean(dto.MotorName),

                    MaxPower =
                        dto.MaxPower,

                    MaxTorque =
                        dto.MaxTorque,

                    MaxPowerRpm =
                        dto.MaxPowerRPM,

                    MaxPowerRpmmin =
                        dto.MaxPowerRPMMin,

                    MaxPowerRpmmax =
                        dto.MaxPowerRPMMax,

                    MaxTorqueRpm =
                        dto.MaxTorqueRPM,

                    MaxTorqueRpmmin =
                        dto.MaxTorqueRPMMin,

                    MaxTorqueRpmmax =
                        dto.MaxTorqueRPMMax,

                    ProductionMotorPerformanceId =
                        null
                };

            _context.ImportMotorPerformances.Add(
                performance);

            await _context.SaveChangesAsync(
                cancellationToken);

            return performance.ImportMotorPerformanceId;
        }

        // ============================================================
        // CREATE TRANSMISSION
        // ============================================================

        public async Task<int> CreateTransmissionAsync(
            CreateStagedTransmissionDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                dto.ImportModelId,
                nameof(dto.ImportModelId));

            var reference =
                Required(
                    dto.TransmissionRef,
                    "TransmissionRef");

            await GetStagedBatchForModelAsync(
                dto.ImportModelId,
                cancellationToken);

            var exists =
                await _context.ImportTransmissions
                    .AnyAsync(
                        x =>
                            x.ImportModelId ==
                                dto.ImportModelId &&
                            x.TransmissionRef ==
                                reference,
                        cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"TransmissionRef '{reference}' already exists for this model.");
            }

            var entity =
                new ImportTransmission
                {
                    ImportModelId =
                        dto.ImportModelId,

                    TransmissionRef =
                        reference,

                    TransmissionType =
                        Clean(dto.TransmissionType),

                    NumberOfGears =
                        dto.NumberOfGears,

                    HasManualOverride =
                        dto.HasManualOverride,

                    HasPaddleShifters =
                        dto.HasPaddleShifters
                };

            _context.ImportTransmissions.Add(entity);

            await _context.SaveChangesAsync(
                cancellationToken);

            return entity.ImportTransmissionId;
        }

        // ============================================================
        // CREATE DRIVETRAIN
        // ============================================================

        public async Task<int> CreateDrivetrainAsync(
            CreateStagedDrivetrainDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                dto.ImportModelId,
                nameof(dto.ImportModelId));

            var reference =
                Required(
                    dto.DrivetrainRef,
                    "DrivetrainRef");

            await GetStagedBatchForModelAsync(
                dto.ImportModelId,
                cancellationToken);

            var exists =
                await _context.ImportDrivetrains
                    .AnyAsync(
                        x =>
                            x.ImportModelId ==
                                dto.ImportModelId &&
                            x.DrivetrainRef ==
                                reference,
                        cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"DrivetrainRef '{reference}' already exists for this model.");
            }

            var entity =
                new ImportDrivetrain
                {
                    ImportModelId =
                        dto.ImportModelId,

                    DrivetrainRef =
                        reference,

                    DrivetrainType =
                        Clean(dto.DrivetrainType),

                    DifferentialType =
                        Clean(dto.DifferentialType)
                };

            _context.ImportDrivetrains.Add(entity);

            await _context.SaveChangesAsync(
                cancellationToken);

            return entity.ImportDrivetrainId;
        }

        // ============================================================
        // CREATE FEATURE
        // ============================================================

        public async Task<int> CreateFeatureAsync(
            CreateStagedFeatureDto dto,
            CancellationToken cancellationToken)
        {
            var importFeature = await _context.ImportFeatures
                .FirstOrDefaultAsync(x =>
                    x.ImportFeatureId == dto.ImportFeatureId);

            if (importFeature == null)
            {
                throw new InvalidOperationException(
                    $"Import feature '{dto.ImportFeatureId}' was not found.");
            }

            if (!string.Equals(
                    importFeature.FeatureCode,
                    dto.FeatureCode,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The selected feature code does not match the selected import feature.");
            }

            var importVariant = await _context.ImportVariants
                .FirstOrDefaultAsync(x =>
                    x.ImportVariantId == dto.ImportVariantId);

            if (importVariant == null)
            {
                throw new InvalidOperationException(
                    $"Import variant '{dto.ImportVariantId}' was not found.");
            }

            var entity = new ImportFeatureVariant
            {
                ImportFeatureId = importFeature.ImportFeatureId,
                ImportVariantId = importVariant.ImportVariantId,
                Available = dto.Available,
                Value = dto.Value,
                SourceColumn = dto.SourceColumn,
                PageNumber = dto.PageNumber,
                Evidence = dto.Evidence,
                Confidence = dto.Confidence
            };

            _context.ImportFeatureVariants.Add(entity);

            await _context.SaveChangesAsync();

            // Add value options separately if supplied.
            if (dto.ValueOptionIds.Count > 0)
            {
                var options = dto.ValueOptionIds
                    .Distinct()
                    .Select(valueOptionId => new ImportFeatureValueOption
                    {
                        ImportFeatureVariantId = entity.ImportFeatureVariantId,
                        ValueOptionId = valueOptionId
                    });

                _context.ImportFeatureValueOptions.AddRange(options);

                await _context.SaveChangesAsync();
            }

            return entity.ImportFeatureVariantId;
        }

        // ============================================================
        // CREATE SPECIFICATION
        // ============================================================

        public async Task<int> CreateSpecificationAsync(
            CreateStagedSpecificationDto dto,
            CancellationToken cancellationToken)
        {
            var importSpecification = await _context.ImportSpecifications
                .FirstOrDefaultAsync(x =>
                    x.ImportSpecificationId == dto.ImportSpecificationId);

            if (importSpecification == null)
            {
                throw new InvalidOperationException(
                    $"Import specification '{dto.ImportSpecificationId}' was not found.");
            }

            if (!string.Equals(
                    importSpecification.SpecificationCode,
                    dto.SpecificationCode,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The selected specification code does not match the selected import specification.");
            }

            var importVariant = await _context.ImportVariants
                .FirstOrDefaultAsync(x =>
                    x.ImportVariantId == dto.ImportVariantId);

            if (importVariant == null)
            {
                throw new InvalidOperationException(
                    $"Import variant '{dto.ImportVariantId}' was not found.");
            }

            var entity = new ImportSpecificationVariant
            {
                ImportSpecificationId =
                    importSpecification.ImportSpecificationId,

                ImportVariantId =
                    importVariant.ImportVariantId,

                NumericValue = dto.NumericValue,
                TextValue = dto.TextValue,
                BooleanValue = dto.BooleanValue,
                Unit = dto.Unit,
                SourceColumn = dto.SourceColumn,
                PageNumber = dto.PageNumber,
                Evidence = dto.Evidence,
                Confidence = dto.Confidence
            };

            _context.ImportSpecificationVariants.Add(entity);

            await _context.SaveChangesAsync();

            return entity.ImportSpecificationVariantId;
        }

        // ============================================================
        // CREATE WARRANTY
        // ============================================================

        public async Task<int> CreateWarrantyAsync(
            CreateStagedWarrantyDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                dto.ImportModelId,
                nameof(dto.ImportModelId));

            await GetStagedBatchForModelAsync(
                dto.ImportModelId,
                cancellationToken);

            int? variantId =
                dto.ImportVariantId;

            if (variantId.HasValue)
            {
                ValidateId(
                    variantId.Value,
                    nameof(dto.ImportVariantId));

                var belongs =
                    await _context.ImportVariants
                        .AnyAsync(
                            x =>
                                x.ImportVariantId ==
                                    variantId.Value &&
                                _context.ImportVariantParents
                                    .Any(p =>
                                        p.ImportVariantParentId ==
                                            x.ImportVariantParentId &&
                                        p.ImportModelId ==
                                            dto.ImportModelId),
                            cancellationToken);

                if (!belongs)
                {
                    throw new InvalidOperationException(
                        $"Variant {variantId.Value} does not belong to model {dto.ImportModelId}.");
                }
            }

            var entity =
                new ImportWarranty
                {
                    ImportModelId =
                        dto.ImportModelId,

                    ImportVariantId =
                        variantId,

                    WarrantyType =
                        Required(
                            dto.WarrantyType,
                            "WarrantyType"),

                    DurationYears =
                        dto.DurationYears,

                    Kilometres =
                        dto.Kilometres,

                    MaximumDurationYears =
                        dto.MaximumDurationYears,

                    MaximumKilometres =
                        dto.MaximumKilometres,

                    SourceId =
                        dto.SourceId,

                    EvidenceText =
                        Clean(dto.EvidenceText),

                    PageNumber =
                        dto.PageNumber,

                    Confidence =
                        dto.Confidence
                };

            _context.ImportWarranties.Add(
                entity);

            await _context.SaveChangesAsync(
                cancellationToken);

            return entity.ImportWarrantyId;
        }

        // ============================================================
        // CREATE MODEL DIMENSIONS
        // ============================================================

        public async Task<int> CreateModelDimensionsAsync(
            int importModelId,
            CreateStagedModelDimensionsDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(
                importModelId,
                nameof(importModelId));

            await GetStagedBatchForModelAsync(
                importModelId,
                cancellationToken);

            var existing =
                await _context.ImportModelDimensions
                    .FirstOrDefaultAsync(
                        x =>
                            x.ImportModelId ==
                            importModelId,
                        cancellationToken);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    $"Model dimensions already exist for staged model {importModelId}. Use the update API instead.");
            }

            var entity =
                new ImportModelDimension
                {
                    ImportModelId =
                        importModelId,

                    LengthMm =
                        dto.LengthMM,

                    WidthMm =
                        dto.WidthMM,

                    HeightMm =
                        dto.HeightMM,

                    WheelbaseMm =
                        dto.WheelbaseMM,

                    GroundClearanceMm =
                        dto.GroundClearanceMM,

                    BootSpaceLitres =
                        dto.BootSpaceLitres,

                    FuelTankCapacityLitres =
                        dto.FuelTankCapacityLitres,

                    SourceId =
                        dto.SourceId,

                    EvidenceText =
                        Clean(dto.EvidenceText),

                    PageNumber =
                        dto.PageNumber,

                    Confidence =
                        dto.Confidence
                };

            _context.ImportModelDimensions.Add(
                entity);

            await _context.SaveChangesAsync(
                cancellationToken);

            return entity.ImportModelDimensionId;
        }

        // ============================================================
        // CREATE FUEL EFFICIENCY
        // ============================================================

        public async Task<int> CreateFuelEfficiencyAsync(
            CreateStagedFuelEfficiencyDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.ImportModelId <= 0)
            {
                throw new ArgumentException(
                    "ImportModelId must be greater than zero.",
                    nameof(dto.ImportModelId));
            }

            var variantIds =
                dto.ImportVariantIds?
                    .Distinct()
                    .ToList()
                ?? [];

            if (variantIds.Count == 0)
            {
                throw new ArgumentException(
                    "At least one variant must be supplied for fuel efficiency.",
                    nameof(dto.ImportVariantIds));
            }

            if (variantIds.Any(x => x <= 0))
            {
                throw new ArgumentException(
                    "All fuel-efficiency variant IDs must be greater than zero.",
                    nameof(dto.ImportVariantIds));
            }

            /*
             * ---------------------------------------------------------
             * 1. Find the staged model and its batch/source.
             * ---------------------------------------------------------
             *
             * ImportModel -> ImportRecord -> ImportBatch
             *
             * ImportRecord.SourceId is the DataSourceId generated
             * for the current brochure import.
             */
            var modelInfo =
                await _context.ImportModels
                    .Where(m =>
                        m.ImportModelId == dto.ImportModelId &&
                        _context.ImportRecords.Any(
                            r =>
                                r.ImportRecordId == m.ImportRecordId &&
                                _context.ImportBatches.Any(
                                    b =>
                                        b.ImportBatchId == r.ImportBatchId &&
                                        b.Status == "Staged")))
                    .Select(m => new
                    {
                        m.ImportModelId,

                        Batch =
                            _context.ImportRecords
                                .Where(r =>
                                    r.ImportRecordId ==
                                        m.ImportRecordId)
                                .Select(r => new
                                {
                                    r.ImportBatchId,
                                    r.SourceId
                                })
                                .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (modelInfo == null)
            {
                throw new InvalidOperationException(
                    $"Staged model {dto.ImportModelId} was not found.");
            }

            if (modelInfo.Batch == null)
            {
                throw new InvalidOperationException(
                    $"No import batch was found for staged model " +
                    $"{dto.ImportModelId}.");
            }

            if (modelInfo.Batch.SourceId == null)
            {
                throw new InvalidOperationException(
                    $"No DataSourceId is associated with the import batch " +
                    $"{modelInfo.Batch.ImportBatchId}.");
            }

            /*
             * ---------------------------------------------------------
             * 2. Validate all variants belong to this model.
             * ---------------------------------------------------------
             */
            var validVariantIds =
                await _context.ImportVariants
                    .Where(v =>
                        variantIds.Contains(v.ImportVariantId) &&
                        _context.ImportVariantParents.Any(
                            p =>
                                p.ImportVariantParentId ==
                                    v.ImportVariantParentId &&
                                p.ImportModelId ==
                                    dto.ImportModelId))
                    .Select(v => v.ImportVariantId)
                    .ToListAsync(
                        cancellationToken);

            if (validVariantIds.Count != variantIds.Count)
            {
                var invalidVariantIds =
                    variantIds
                        .Except(validVariantIds)
                        .ToList();

                throw new InvalidOperationException(
                    "One or more supplied variants do not belong to " +
                    $"staged model {dto.ImportModelId}. " +
                    $"Invalid variant IDs: {string.Join(", ", invalidVariantIds)}.");
            }

            /*
             * ---------------------------------------------------------
             * 3. Create ONE fuel-efficiency record.
             *
             * This represents ONE source/value.
             *
             * Example:
             *
             *   19 km/l   Manufacturer
             *
             * It can then be attached to multiple variants.
             * ---------------------------------------------------------
             */
            var entity =
                new ImportFuelEfficiency
                {
                    FuelEfficiency =
                        dto.FuelEfficiency,

                    FuelEfficiencyUnit =
                        Required(
                            dto.FuelEfficiencyUnit,
                            "FuelEfficiencyUnit"),

                    SourceType =
                        "Manufacturer",

                    IsActive =
                        true,

                    SourceId =
                        modelInfo.Batch.SourceId.Value,

                    SourceColumn =
                        Clean(dto.SourceColumn),

                    PageNumber =
                        dto.PageNumber,

                    Evidence =
                        dto.Evidence,

                    Confidence =
                        dto.Confidence,

                    ProductionFuelEfficiencyId =
                        null
                };

            _context.ImportFuelEfficiencies.Add(entity);

            await _context.SaveChangesAsync(
                cancellationToken);

            /*
             * ---------------------------------------------------------
             * 4. Attach the same fuel-efficiency record to every
             *    supplied variant.
             * ---------------------------------------------------------
             */
            foreach (var variantId in variantIds)
            {
                _context.ImportFuelEfficiencyVariants.Add(
                    new ImportFuelEfficiencyVariant
                    {
                        ImportFuelEfficiencyId =
                            entity.ImportFuelEfficiencyId,

                        ImportVariantId =
                            variantId
                    });
            }

            await _context.SaveChangesAsync(
                cancellationToken);

            return entity.ImportFuelEfficiencyId;
        }

        // ============================================================
        // CREATE FUEL TYPE
        // ============================================================
        //
        // There is intentionally no ImportFuelType table.
        // FuelTypeName is stored on ImportEnginePerformance and
        // FuelTypeId remains NULL during staging.
        //
        // Therefore a genuinely new fuel type must be created in
        // the production FuelType master so that the later production
        // import can resolve FuelTypeName -> FuelTypeId.
        //
        // ============================================================

        public async Task<int> CreateFuelTypeAsync(
            CreateStagedFuelTypeDto dto,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var fuelTypeName =
                Required(
                    dto.FuelTypeName,
                    "FuelTypeName");

            var existing =
                await _context.FuelTypes
                    .FirstOrDefaultAsync(
                        x =>
                            x.FuelType1 == fuelTypeName,
                        cancellationToken);

            if (existing != null)
            {
                return existing.FuelTypeId;
            }

            var fuelType =
                new FuelType
                {
                    FuelType1 =
                        fuelTypeName
                };

            _context.FuelTypes.Add(
                fuelType);

            await _context.SaveChangesAsync(
                cancellationToken);

            return fuelType.FuelTypeId;
        }

        // ============================================================
        // ATTACH NEWLY CREATED FUEL EFFICIENCY TO VARIANT
        // ============================================================

        public async Task AttachFuelEfficiencyToVariantAsync(
            int importFuelEfficiencyId,
            int importVariantId,
            CancellationToken cancellationToken)
        {
            ValidateId(
                importFuelEfficiencyId,
                nameof(importFuelEfficiencyId));

            ValidateId(
                importVariantId,
                nameof(importVariantId));

            var modelId =
                await GetModelIdForVariantAsync(
                    importVariantId,
                    cancellationToken);

            await GetStagedBatchForModelAsync(
                modelId,
                cancellationToken);

            var fuelEfficiencyExists =
                await _context.ImportFuelEfficiencies
                    .AnyAsync(
                        x =>
                            x.ImportFuelEfficiencyId ==
                            importFuelEfficiencyId,
                        cancellationToken);

            if (!fuelEfficiencyExists)
            {
                throw new KeyNotFoundException(
                    $"Staged fuel efficiency {importFuelEfficiencyId} was not found.");
            }

            var existing =
                await _context.ImportFuelEfficiencyVariants
                    .AnyAsync(
                        x =>
                            x.ImportFuelEfficiencyId ==
                                importFuelEfficiencyId &&
                            x.ImportVariantId ==
                                importVariantId,
                        cancellationToken);

            if (existing)
            {
                throw new InvalidOperationException(
                    $"Fuel efficiency {importFuelEfficiencyId} is already attached to variant {importVariantId}.");
            }

            _context.ImportFuelEfficiencyVariants.Add(
                new ImportFuelEfficiencyVariant
                {
                    ImportFuelEfficiencyId =
                        importFuelEfficiencyId,

                    ImportVariantId =
                        importVariantId
                });

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<StagedPowertrainDependenciesDto> GetPowertrainDependenciesAsync(
        CancellationToken cancellationToken = default)
        {
            var engines = await _context.ImportEngines
                .AsNoTracking()
                .OrderBy(x => x.EngineRef)
                .ThenBy(x => x.ImportEngineId)
                .Select(x => new StagedEngineLookupDto
                {
                    ImportEngineId = x.ImportEngineId,
                    ImportPowertrainId = x.ImportPowertrainId,

                    EngineRef = x.EngineRef,
                    EngineName = x.EngineName,

                    NumberOfCylinders = x.NumberOfCylinders,
                    NumberOfValves = x.NumberOfValves,
                    Displacement = x.Displacement,
                    IsTurbocharged = x.IsTurbocharged,

                    EmissionStandard = x.EmissionStandard,
                    Aspiration = x.Aspiration,
                    EngineType = x.EngineType,

                    ProductionEngineId = x.ProductionEngineId
                })
                .ToListAsync(cancellationToken);


            var enginePerformances = await _context.ImportEnginePerformances
                .AsNoTracking()
                .OrderBy(x => x.ImportEngineId)
                .ThenBy(x => x.ModeName)
                .ThenBy(x => x.ImportEnginePerformanceId)
                .Select(x => new StagedEnginePerformanceLookupDto
                {
                    ImportEnginePerformanceId =
                        x.ImportEnginePerformanceId,

                    ImportEngineId =
                        x.ImportEngineId,

                    EngineRef =
                        x.ImportEngine.EngineRef,

                    EngineName =
                        x.ImportEngine.EngineName,

                    ModeName =
                        x.ModeName,

                    FuelTypeId =
                        x.FuelTypeId,

                    FuelTypeName =
                        x.FuelType != null
                            ? x.FuelType.FuelType1
                            : null,

                    MaxPower =
                        x.MaxPower,

                    MaxTorque =
                        x.MaxTorque,

                    MaxPowerRPM =
                        x.MaxPowerRpm,

                    MaxPowerRPMMin =
                        x.MaxPowerRpmmin,

                    MaxPowerRPMMax =
                        x.MaxPowerRpmmax,

                    MaxTorqueRPM =
                        x.MaxTorqueRpm,

                    MaxTorqueRPMMin =
                        x.MaxTorqueRpmmin,

                    MaxTorqueRPMMax =
                        x.MaxTorqueRpmmax,

                    ProductionEnginePerformanceId =
                        x.ProductionEnginePerformanceId
                })
                .ToListAsync(cancellationToken);


            var motorPerformances = await _context.ImportMotorPerformances
                .AsNoTracking()
                .OrderBy(x => x.ImportPowertrainId)
                .ThenBy(x => x.MotorName)
                .ThenBy(x => x.ImportMotorPerformanceId)
                .Select(x => new StagedMotorPerformanceLookupDto
                {
                    ImportMotorPerformanceId =
                        x.ImportMotorPerformanceId,

                    ImportPowertrainId =
                        x.ImportPowertrainId,

                    MotorName =
                        x.MotorName,

                    MaxPower =
                        x.MaxPower,

                    MaxTorque =
                        x.MaxTorque,

                    MaxPowerRPM =
                        x.MaxPowerRpm,

                    MaxPowerRPMMin =
                        x.MaxPowerRpmmin,

                    MaxPowerRPMMax =
                        x.MaxPowerRpmmax,

                    MaxTorqueRPM =
                        x.MaxTorqueRpm,

                    MaxTorqueRPMMin =
                        x.MaxTorqueRpmmin,

                    MaxTorqueRPMMax =
                        x.MaxTorqueRpmmax,

                    ProductionMotorPerformanceId =
                        x.ProductionMotorPerformanceId
                })
                .ToListAsync(cancellationToken);


            return new StagedPowertrainDependenciesDto
            {
                Engines = engines,
                EnginePerformances = enginePerformances,
                MotorPerformances = motorPerformances
            };
        }

        public async Task<StagedFeatureSpecificationCatalogDto> GetFeatureSpecificationCatalogAsync(CancellationToken cancellationToken = default)
        {
            var features = await _context.ImportFeatures
           .AsNoTracking()
           .Where(x => !string.IsNullOrWhiteSpace(x.FeatureCode))
           .GroupBy(x => new
           {
               x.FeatureCode,
               x.FeatureId
           })
           .Select(g => new StagedFeatureCatalogDto
           {
               ImportFeatureId = g.Min(x => x.ImportFeatureId),
               FeatureCode = g.Key.FeatureCode,
               FeatureName = g
                   .Select(x => x.Feature != null
                       ? x.Feature.FeatureName
                       : null)
                   .FirstOrDefault()
           })
           .OrderBy(x => x.FeatureCode)
           .ToListAsync();

            var specifications = await _context.ImportSpecifications
                .AsNoTracking()
                .Where(x => !string.IsNullOrWhiteSpace(x.SpecificationCode))
                .GroupBy(x => new
                {
                    x.SpecificationCode,
                    x.SpecificationId
                })
                .Select(g => new StagedSpecificationCatalogDto
                {
                    ImportSpecificationId = g.Min(x => x.ImportSpecificationId),
                    SpecificationCode = g.Key.SpecificationCode,
                    SpecificationName = g
                        .Select(x => x.Specification != null
                            ? x.Specification.SpecificationName
                            : null)
                        .FirstOrDefault()
                })
                .OrderBy(x => x.SpecificationCode)
                .ToListAsync();

            return new StagedFeatureSpecificationCatalogDto
            {
                Features = features,
                Specifications = specifications
            };

        }

        // ============================================================
        // POWERTRAIN LOOKUP
        // ============================================================

        public async Task<List<StagedPowertrainLookupDto>>
            GetPowertrainsForModelAsync(
                int importModelId)
        {
            var modelExists =
                await _context.ImportModels
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.ImportModelId == importModelId);

            if (!modelExists)
                throw new KeyNotFoundException(
                    "Staged model not found.");

            return await _context.ImportPowertrains
                .AsNoTracking()
                .Where(x =>
                    x.ImportModelId == importModelId)
                .OrderBy(x => x.PowertrainRef)
                .Select(x =>
                    new StagedPowertrainLookupDto
                    {
                        ImportPowertrainId =
                            x.ImportPowertrainId,

                        ImportModelId =
                            x.ImportModelId,

                        PowertrainRef =
                            x.PowertrainRef,

                        PowertrainType =
                            x.PowertrainType,

                        EngineRef =
                            x.EngineRef,

                        BatteryCapacityKWh =
                            x.BatteryCapacityKwh,

                        CombinedMaxPower =
                            x.CombinedMaxPower,

                        CombinedMaxPowerUnit =
                            x.CombinedMaxPowerUnit,

                        CombinedMaxTorque =
                            x.CombinedMaxTorque,

                        CombinedMaxTorqueUnit =
                            x.CombinedMaxTorqueUnit,

                        CombinedMaxPowerRPM =
                            x.CombinedMaxPowerRpm,

                        CombinedMaxPowerRPMMin =
                            x.CombinedMaxPowerRpmmin,

                        CombinedMaxPowerRPMMax =
                            x.CombinedMaxPowerRpmmax,

                        CombinedMaxTorqueRPM =
                            x.CombinedMaxTorqueRpm,

                        CombinedMaxTorqueRPMMin =
                            x.CombinedMaxTorqueRpmmin,

                        CombinedMaxTorqueRPMMax =
                            x.CombinedMaxTorqueRpmmax,

                        ProductionPowertrainId =
                            x.ProductionPowertrainId
                    })
                .ToListAsync();
        }


        // ============================================================
        // TRANSMISSION LOOKUP
        // ============================================================

        public async Task<List<StagedTransmissionLookupDto>>
            GetTransmissionsForModelAsync(
                int importModelId)
        {
            var modelExists =
                await _context.ImportModels
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.ImportModelId == importModelId);

            if (!modelExists)
                throw new KeyNotFoundException(
                    "Staged model not found.");

            return await _context.ImportTransmissions
                .AsNoTracking()
                .Where(x =>
                    x.ImportModelId == importModelId)
                .OrderBy(x => x.TransmissionRef)
                .Select(x =>
                    new StagedTransmissionLookupDto
                    {
                        ImportTransmissionId =
                            x.ImportTransmissionId,

                        ImportModelId =
                            x.ImportModelId,

                        TransmissionRef =
                            x.TransmissionRef,

                        TransmissionType =
                            x.TransmissionType,

                        NumberOfGears =
                            x.NumberOfGears,

                        HasManualOverride =
                            x.HasManualOverride,

                        HasPaddleShifters =
                            x.HasPaddleShifters,

                        ProductionTransmissionId =
                            x.ProductionTransmissionId
                    })
                .ToListAsync();
        }


        // ============================================================
        // DRIVETRAIN LOOKUP
        // ============================================================

        public async Task<List<StagedDrivetrainLookupDto>>
            GetDrivetrainsForModelAsync(
                int importModelId)
        {
            var modelExists =
                await _context.ImportModels
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.ImportModelId == importModelId);

            if (!modelExists)
                throw new KeyNotFoundException(
                    "Staged model not found.");

            return await _context.ImportDrivetrains
                .AsNoTracking()
                .Where(x =>
                    x.ImportModelId == importModelId)
                .OrderBy(x => x.DrivetrainRef)
                .Select(x =>
                    new StagedDrivetrainLookupDto
                    {
                        ImportDrivetrainId =
                            x.ImportDrivetrainId,

                        ImportModelId =
                            x.ImportModelId,

                        DrivetrainRef =
                            x.DrivetrainRef,

                        DrivetrainType =
                            x.DrivetrainType,

                        DifferentialType =
                            x.DifferentialType,

                        ProductionDrivetrainId =
                            x.ProductionDrivetrainId
                    })
                .ToListAsync();
        }


        // ============================================================
        // REPLACE POWERTRAIN REFERENCE
        // ============================================================

        public async Task<ReplaceStagedPowertrainReferenceResultDto>
            ReplaceVariantPowertrainReferenceAsync(
                int importVariantId,
                ReplaceStagedPowertrainReferenceDto dto)
        {
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var variant =
                    await _context.ImportVariants.Include(x => x.ImportVariantParent)
                        .FirstOrDefaultAsync(x =>
                            x.ImportVariantId ==
                            importVariantId);

                if (variant == null)
                    throw new KeyNotFoundException(
                        "Staged variant not found.");

                var newPowertrain =
                    await _context.ImportPowertrains
                        .FirstOrDefaultAsync(x =>
                            x.ImportPowertrainId ==
                            dto.ImportPowertrainId);

                if (newPowertrain == null)
                    throw new KeyNotFoundException(
                        "Selected powertrain not found.");

                if (
                    newPowertrain.ImportModelId !=
                    variant.ImportVariantParent.ImportModelId)
                {
                    throw new InvalidOperationException(
                        "The selected powertrain does not belong to the same staged model as the variant.");
                }

                var previousRef =
                    variant.PowertrainRef;

                var previousPowertrain =
                    !string.IsNullOrWhiteSpace(previousRef)
                        ? await _context.ImportPowertrains
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x =>
                                x.ImportModelId ==
                                    variant.ImportVariantParent.ImportModelId &&
                                x.PowertrainRef ==
                                    previousRef)
                        : null;

                if (
                    previousPowertrain != null &&
                    previousPowertrain.ImportPowertrainId ==
                        newPowertrain.ImportPowertrainId)
                {
                    throw new InvalidOperationException(
                        "The selected powertrain is already referenced by this variant.");
                }

                variant.PowertrainRef =
                    newPowertrain.PowertrainRef;
                variant.ImportPowertrainId = newPowertrain.ImportPowertrainId;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ReplaceStagedPowertrainReferenceResultDto
                {
                    ImportVariantId =
                        variant.ImportVariantId,

                    PreviousImportPowertrainId =
                        previousPowertrain?.ImportPowertrainId,

                    NewImportPowertrainId =
                        newPowertrain.ImportPowertrainId,

                    PreviousPowertrainRef =
                        previousRef,

                    NewPowertrainRef =
                        newPowertrain.PowertrainRef,

                    Message =
                        "Powertrain reference replaced successfully."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        // ============================================================
        // REPLACE TRANSMISSION REFERENCE
        // ============================================================

        public async Task<ReplaceStagedTransmissionReferenceResultDto>
            ReplaceVariantTransmissionReferenceAsync(
                int importVariantId,
                ReplaceStagedTransmissionReferenceDto dto)
        {
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var variant =
                    await _context.ImportVariants.Include(x => x.ImportVariantParent)
                        .FirstOrDefaultAsync(x =>
                            x.ImportVariantId ==
                            importVariantId);

                if (variant == null)
                    throw new KeyNotFoundException(
                        "Staged variant not found.");

                var newTransmission =
                    await _context.ImportTransmissions
                        .FirstOrDefaultAsync(x =>
                            x.ImportTransmissionId ==
                            dto.ImportTransmissionId);

                if (newTransmission == null)
                    throw new KeyNotFoundException(
                        "Selected transmission not found.");

                if (
                    newTransmission.ImportModelId !=
                    variant.ImportVariantParent.ImportModelId)
                {
                    throw new InvalidOperationException(
                        "The selected transmission does not belong to the same staged model as the variant.");
                }

                var previousRef =
                    variant.TransmissionRef;

                var previousTransmission =
                    !string.IsNullOrWhiteSpace(previousRef)
                        ? await _context.ImportTransmissions
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x =>
                                x.ImportModelId ==
                                    variant.ImportVariantParent.ImportModelId &&
                                x.TransmissionRef ==
                                    previousRef)
                        : null;

                if (
                    previousTransmission != null &&
                    previousTransmission.ImportTransmissionId ==
                        newTransmission.ImportTransmissionId)
                {
                    throw new InvalidOperationException(
                        "The selected transmission is already referenced by this variant.");
                }

                variant.TransmissionRef =
                    newTransmission.TransmissionRef;
                variant.ImportTransmissionId =
                    newTransmission.ImportTransmissionId;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ReplaceStagedTransmissionReferenceResultDto
                {
                    ImportVariantId =
                        variant.ImportVariantId,

                    PreviousImportTransmissionId =
                        previousTransmission?.ImportTransmissionId,

                    NewImportTransmissionId =
                        newTransmission.ImportTransmissionId,

                    PreviousTransmissionRef =
                        previousRef,

                    NewTransmissionRef =
                        newTransmission.TransmissionRef,

                    Message =
                        "Transmission reference replaced successfully."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        // ============================================================
        // REPLACE DRIVETRAIN REFERENCE
        // ============================================================

        public async Task<ReplaceStagedDrivetrainReferenceResultDto>
            ReplaceVariantDrivetrainReferenceAsync(
                int importVariantId,
                ReplaceStagedDrivetrainReferenceDto dto)
        {
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var variant =
                    await _context.ImportVariants.Include(x => x.ImportVariantParent)
                        .FirstOrDefaultAsync(x =>
                            x.ImportVariantId ==
                            importVariantId);

                if (variant == null)
                    throw new KeyNotFoundException(
                        "Staged variant not found.");

                var newDrivetrain =
                    await _context.ImportDrivetrains
                        .FirstOrDefaultAsync(x =>
                            x.ImportDrivetrainId ==
                            dto.ImportDrivetrainId);

                if (newDrivetrain == null)
                    throw new KeyNotFoundException(
                        "Selected drivetrain not found.");

                if (
                    newDrivetrain.ImportModelId !=
                    variant.ImportVariantParent.ImportModelId)
                {
                    throw new InvalidOperationException(
                        "The selected drivetrain does not belong to the same staged model as the variant.");
                }

                var previousRef =
                    variant.DrivetrainRef;
                

                var previousDrivetrain =
                    !string.IsNullOrWhiteSpace(previousRef)
                        ? await _context.ImportDrivetrains
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x =>
                                x.ImportModelId ==
                                    variant.ImportVariantParent.ImportModelId &&
                                x.DrivetrainRef ==
                                    previousRef)
                        : null;

                if (
                    previousDrivetrain != null &&
                    previousDrivetrain.ImportDrivetrainId ==
                        newDrivetrain.ImportDrivetrainId)
                {
                    throw new InvalidOperationException(
                        "The selected drivetrain is already referenced by this variant.");
                }

                variant.DrivetrainRef =
                    newDrivetrain.DrivetrainRef;
                variant.ImportDrivetrainId = newDrivetrain.ImportDrivetrainId;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ReplaceStagedDrivetrainReferenceResultDto
                {
                    ImportVariantId =
                        variant.ImportVariantId,

                    PreviousImportDrivetrainId =
                        previousDrivetrain?.ImportDrivetrainId,

                    NewImportDrivetrainId =
                        newDrivetrain.ImportDrivetrainId,

                    PreviousDrivetrainRef =
                        previousRef,

                    NewDrivetrainRef =
                        newDrivetrain.DrivetrainRef,

                    Message =
                        "Drivetrain reference replaced successfully."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            
        }
    }
}