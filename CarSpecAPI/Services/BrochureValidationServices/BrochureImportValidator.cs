using CarSpecAPI.Data;
using CarSpecAPI.Data.Models.DataExtractionModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Services.BrochureValidationServices
{
    public sealed class BrochureImportValidator
    {
        private readonly CarsDbContext carsDbContext;

        public BrochureImportValidator(
            CarsDbContext db)
        {
            carsDbContext = db;
        }

        public async Task<BrochureImportValidationResult> ValidateAsync(
            BrochureExtractionDto data,
            CancellationToken cancellationToken = default)
        {
            var result = new BrochureImportValidationResult();

            if (data == null)
            {
                result.Errors.Add("Brochure extraction data is null.");
                return result;
            }

            ValidateVehicle(
                data,
                result);

            ValidatePowertrains(
                data,
                result);

            ValidateEngines(
                data,
                result);

            ValidateTransmissions(
                data,
                result);

            ValidateDrivetrains(
                data,
                result);

            ValidateVariants(
                data,
                result);

            ValidateReferences(
                data,
                result);

            ValidateFeatures(
                data,
                result);

            ValidateSpecifications(
                data,
                result);

            ValidateFuelEfficiency(
                data,
                result);

            await ValidateCatalogReferencesAsync(
                data,
                result,
                cancellationToken);

            return result;
        }

        // ================================================================
        // VEHICLE
        // ================================================================

        private static void ValidateVehicle(
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            if (data.Vehicle == null)
            {
                result.Errors.Add(
                    "vehicle is missing.");

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    data.Vehicle.Brand))
            {
                result.Errors.Add(
                    "vehicle.brand is missing.");
            }

            if (string.IsNullOrWhiteSpace(
                    data.Vehicle.ModelName))
            {
                result.Errors.Add(
                    "vehicle.modelName is missing.");
            }
        }

        // ================================================================
        // POWERTRAINS
        // ================================================================

        private static void ValidatePowertrains(
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            var duplicates =
                data.Powertrains
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.PowertrainRef))
                    .GroupBy(
                        x => x.PowertrainRef.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Where(x => x.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                result.Errors.Add(
                    $"Duplicate powertrainRef '{duplicate.Key}'.");
            }

            foreach (var powertrain in data.Powertrains)
            {
                if (string.IsNullOrWhiteSpace(
                        powertrain.PowertrainRef))
                {
                    result.Errors.Add(
                        "Powertrain has empty powertrainRef.");
                }

                if (string.IsNullOrWhiteSpace(
                        powertrain.PowertrainType))
                {
                    result.Errors.Add(
                        $"Powertrain '{powertrain.PowertrainRef}' " +
                        "has no powertrainType.");
                }
                else if (!new[]
                {
                    "ICE",
                    "EV",
                    "Hybrid"
                }.Contains(
                    powertrain.PowertrainType,
                    StringComparer.OrdinalIgnoreCase))
                {
                    result.Errors.Add(
                        $"Powertrain '{powertrain.PowertrainRef}' " +
                        $"has invalid type '{powertrain.PowertrainType}'.");
                }

                if (powertrain.MotorPerformances == null)
                {
                    continue;
                }
            }
        }

        // ================================================================
        // ENGINES
        // ================================================================

        private static void ValidateEngines(
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            var duplicates =
                data.Engines
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.EngineRef))
                    .GroupBy(
                        x => x.EngineRef.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Where(x => x.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                result.Errors.Add(
                    $"Duplicate engineRef '{duplicate.Key}'.");
            }

            foreach (var engine in data.Engines)
            {
                if (string.IsNullOrWhiteSpace(
                        engine.EngineRef))
                {
                    result.Errors.Add(
                        "Engine has empty engineRef.");
                }

                if (string.IsNullOrWhiteSpace(
                        engine.EngineName))
                {
                    result.Errors.Add(
                        $"Engine '{engine.EngineRef}' has no engineName.");
                }

                if (engine.EnginePerformances == null)
                {
                    continue;
                }

                foreach (var performance
                         in engine.EnginePerformances)
                {
                    if (string.IsNullOrWhiteSpace(
                            performance.ModeName))
                    {
                        result.Errors.Add(
                            $"Engine '{engine.EngineRef}' " +
                            "has engine performance without modeName.");
                    }
                }
            }
        }

        // ================================================================
        // TRANSMISSIONS
        // ================================================================

        private static void ValidateTransmissions(
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            var duplicates =
                data.Transmissions
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.TransmissionRef))
                    .GroupBy(
                        x => x.TransmissionRef.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Where(x => x.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                result.Errors.Add(
                    $"Duplicate transmissionRef '{duplicate.Key}'.");
            }

            foreach (var transmission in data.Transmissions)
            {
                if (string.IsNullOrWhiteSpace(
                        transmission.TransmissionRef))
                {
                    result.Errors.Add(
                        "Transmission has empty transmissionRef.");
                }
            }
        }

        // ================================================================
        // DRIVETRAINS
        // ================================================================

        private static void ValidateDrivetrains(
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            var duplicates =
                data.Drivetrains
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.DrivetrainRef))
                    .GroupBy(
                        x => x.DrivetrainRef.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Where(x => x.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                result.Errors.Add(
                    $"Duplicate drivetrainRef '{duplicate.Key}'.");
            }

            foreach (var drivetrain in data.Drivetrains)
            {
                if (string.IsNullOrWhiteSpace(
                        drivetrain.DrivetrainRef))
                {
                    result.Errors.Add(
                        "Drivetrain has empty drivetrainRef.");
                }
            }
        }

        // ================================================================
        // VARIANTS
        // ================================================================

        private static void ValidateVariants(
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            var duplicateVariants =
                data.Variants
                    .Where(v =>
                        !string.IsNullOrWhiteSpace(
                            v.VariantName))
                    .GroupBy(
                        v => v.VariantName.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Where(x => x.Count() > 1);

            foreach (var duplicate in duplicateVariants)
            {
                result.Errors.Add(
                    $"Duplicate variantName '{duplicate.Key}'.");
            }

            var parents =
                data.Variants
                    .Where(v =>
                        string.Equals(
                            v.VariantType,
                            "parent",
                            StringComparison.OrdinalIgnoreCase) &&
                        !string.IsNullOrWhiteSpace(
                            v.VariantName))
                    .GroupBy(
                        v => v.VariantName.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First(),
                        StringComparer.OrdinalIgnoreCase);

            foreach (var variant in data.Variants)
            {
                if (string.IsNullOrWhiteSpace(
                        variant.VariantName))
                {
                    result.Errors.Add(
                        "Variant has empty variantName.");

                    continue;
                }

                if (string.Equals(
                        variant.VariantType,
                        "parent",
                        StringComparison.OrdinalIgnoreCase))
                {
                    ValidateParentVariant(
                        variant,
                        data,
                        result);
                }
                else if (string.Equals(
                             variant.VariantType,
                             "subVariant",
                             StringComparison.OrdinalIgnoreCase))
                {
                    ValidateSubVariant(
                        variant,
                        parents,
                        result);
                }
                else if (string.Equals(
                             variant.VariantType,
                             "edition",
                             StringComparison.OrdinalIgnoreCase))
                {
                    ValidateEditionVariant(
                        variant,
                        data,
                        result);
                }
                else
                {
                    result.Errors.Add(
                        $"Variant '{variant.VariantName}' " +
                        $"has invalid variantType '{variant.VariantType}'.");
                }
            }

            // Every parent must have at least one concrete child.
            foreach (var parent in parents.Values)
            {
                var hasChild =
                    data.Variants.Any(v =>
                        string.Equals(
                            v.VariantType,
                            "subVariant",
                            StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(
                            v.ParentVariantName,
                            parent.VariantName,
                            StringComparison.OrdinalIgnoreCase));

                if (!hasChild)
                {
                    result.Errors.Add(
                        $"Parent variant '{parent.VariantName}' " +
                        "has no subVariant.");
                }
            }

            // There must be at least one parent.
            if (parents.Count == 0 &&
                data.Variants.Count > 0)
            {
                result.Errors.Add(
                    "No parent variants were found.");
            }
        }

        private static void ValidateParentVariant(
            VariantDto variant,
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            if (variant.ParentVariantName != null)
            {
                result.Errors.Add(
                    $"Parent variant '{variant.VariantName}' " +
                    "must have parentVariantName = null.");
            }

            if (variant.BaseVariantName != null)
            {
                result.Errors.Add(
                    $"Parent variant '{variant.VariantName}' " +
                    "must have baseVariantName = null.");
            }

            if (variant.PowertrainRef != null)
            {
                result.Errors.Add(
                    $"Parent variant '{variant.VariantName}' " +
                    "must not have powertrainRef.");
            }

            if (variant.TransmissionRef != null)
            {
                result.Errors.Add(
                    $"Parent variant '{variant.VariantName}' " +
                    "must not have transmissionRef.");
            }

            if (variant.DrivetrainRef != null)
            {
                result.Errors.Add(
                    $"Parent variant '{variant.VariantName}' " +
                    "must not have drivetrainRef.");
            }
        }

        private static void ValidateSubVariant(
            VariantDto variant,
            Dictionary<string, VariantDto> parents,
            BrochureImportValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(
                    variant.ParentVariantName))
            {
                result.Errors.Add(
                    $"SubVariant '{variant.VariantName}' " +
                    "has no parentVariantName.");
            }
            else if (!parents.ContainsKey(
                         variant.ParentVariantName))
            {
                result.Errors.Add(
                    $"SubVariant '{variant.VariantName}' " +
                    $"references missing parent " +
                    $"'{variant.ParentVariantName}'.");
            }

            if (string.IsNullOrWhiteSpace(
                    variant.PowertrainRef))
            {
                result.Errors.Add(
                    $"SubVariant '{variant.VariantName}' " +
                    "has no powertrainRef.");
            }

            if (string.IsNullOrWhiteSpace(
                    variant.TransmissionRef))
            {
                result.Errors.Add(
                    $"SubVariant '{variant.VariantName}' " +
                    "has no transmissionRef.");
            }

            if (string.IsNullOrWhiteSpace(
                    variant.DrivetrainRef))
            {
                result.Errors.Add(
                    $"SubVariant '{variant.VariantName}' " +
                    "has no drivetrainRef.");
            }

            if (variant.BaseVariantName != null)
            {
                result.Errors.Add(
                    $"SubVariant '{variant.VariantName}' " +
                    "must have baseVariantName = null.");
            }
        }

        private static void ValidateEditionVariant(
            VariantDto variant,
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(
                    variant.BaseVariantName))
            {
                result.Errors.Add(
                    $"Edition '{variant.VariantName}' " +
                    "has no baseVariantName.");

                return;
            }

            var baseVariantExists =
                data.Variants.Any(v =>
                    string.Equals(
                        v.VariantName,
                        variant.BaseVariantName,
                        StringComparison.OrdinalIgnoreCase) &&
                    (
                        string.Equals(
                            v.VariantType,
                            "parent",
                            StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(
                            v.VariantType,
                            "subVariant",
                            StringComparison.OrdinalIgnoreCase)
                    ));

            if (!baseVariantExists)
            {
                result.Errors.Add(
                    $"Edition '{variant.VariantName}' " +
                    $"references missing base variant " +
                    $"'{variant.BaseVariantName}'.");
            }

            if (variant.ParentVariantName != null)
            {
                result.Errors.Add(
                    $"Edition '{variant.VariantName}' " +
                    "must have parentVariantName = null.");
            }

            if (variant.PowertrainRef != null ||
                variant.TransmissionRef != null ||
                variant.DrivetrainRef != null)
            {
                result.Errors.Add(
                    $"Edition '{variant.VariantName}' " +
                    "must not contain configuration references.");
            }
        }

        // ================================================================
        // REFERENCES
        // ================================================================

        private static void ValidateReferences(
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            var powertrainRefs =
                data.Powertrains
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.PowertrainRef))
                    .Select(x => x.PowertrainRef.Trim())
                    .ToHashSet(
                        StringComparer.OrdinalIgnoreCase);

            var transmissionRefs =
                data.Transmissions
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.TransmissionRef))
                    .Select(x => x.TransmissionRef.Trim())
                    .ToHashSet(
                        StringComparer.OrdinalIgnoreCase);

            var drivetrainRefs =
                data.Drivetrains
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.DrivetrainRef))
                    .Select(x => x.DrivetrainRef.Trim())
                    .ToHashSet(
                        StringComparer.OrdinalIgnoreCase);

            var engineRefs =
                data.Engines
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.EngineRef))
                    .Select(x => x.EngineRef.Trim())
                    .ToHashSet(
                        StringComparer.OrdinalIgnoreCase);

            foreach (var variant in data.Variants)
            {
                if (!string.Equals(
                        variant.VariantType,
                        "subVariant",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(
                        variant.PowertrainRef) &&
                    !powertrainRefs.Contains(
                        variant.PowertrainRef.Trim()))
                {
                    result.Errors.Add(
                        $"Variant '{variant.VariantName}' " +
                        $"references unknown powertrain " +
                        $"'{variant.PowertrainRef}'.");
                }

                if (!string.IsNullOrWhiteSpace(
                        variant.TransmissionRef) &&
                    !transmissionRefs.Contains(
                        variant.TransmissionRef.Trim()))
                {
                    result.Errors.Add(
                        $"Variant '{variant.VariantName}' " +
                        $"references unknown transmission " +
                        $"'{variant.TransmissionRef}'.");
                }

                if (!string.IsNullOrWhiteSpace(
                        variant.DrivetrainRef) &&
                    !drivetrainRefs.Contains(
                        variant.DrivetrainRef.Trim()))
                {
                    result.Errors.Add(
                        $"Variant '{variant.VariantName}' " +
                        $"references unknown drivetrain " +
                        $"'{variant.DrivetrainRef}'.");
                }
            }

            foreach (var powertrain in data.Powertrains)
            {
                if (string.IsNullOrWhiteSpace(
                        powertrain.EngineRef))
                {
                    continue;
                }

                if (!engineRefs.Contains(
                        powertrain.EngineRef.Trim()))
                {
                    result.Errors.Add(
                        $"Powertrain '{powertrain.PowertrainRef}' " +
                        $"references missing engine " +
                        $"'{powertrain.EngineRef}'.");
                }
            }

            // Every engine must be referenced by at least one powertrain.
            foreach (var engine in data.Engines)
            {
                var referenced =
                    data.Powertrains.Any(p =>
                        string.Equals(
                            p.EngineRef,
                            engine.EngineRef,
                            StringComparison.OrdinalIgnoreCase));

                if (!referenced)
                {
                    result.Warnings.Add(
                        $"Engine '{engine.EngineRef}' " +
                        "is not referenced by any powertrain.");
                }
            }
        }

        // ================================================================
        // FEATURES
        // ================================================================

        private static void ValidateFeatures(
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            foreach (var feature in data.Features)
            {
                if (feature.FeatureId <= 0)
                {
                    result.Errors.Add(
                        $"Feature '{feature.FeatureCode}' " +
                        "has invalid featureId.");
                }

                if (string.IsNullOrWhiteSpace(
                        feature.FeatureCode))
                {
                    result.Errors.Add(
                        $"FeatureId {feature.FeatureId} " +
                        "has no featureCode.");
                }

                if (feature.ValueOptionIds != null)
                {
                    var duplicateOptionIds =
                        feature.ValueOptionIds
                            .GroupBy(x => x)
                            .Where(x => x.Count() > 1)
                            .Select(x => x.Key);

                    foreach (var optionId in duplicateOptionIds)
                    {
                        result.Warnings.Add(
                            $"Feature '{feature.FeatureCode}' " +
                            $"contains duplicate valueOptionId {optionId}.");
                    }
                }

                if (feature.AppliesToVariants == null)
                {
                    continue;
                }

                foreach (var variantName
                         in feature.AppliesToVariants
                             .Where(x =>
                                 !string.IsNullOrWhiteSpace(x))
                             .Distinct(
                                 StringComparer.OrdinalIgnoreCase))
                {
                    var exists =
                        data.Variants.Any(v =>
                            string.Equals(
                                v.VariantName,
                                variantName,
                                StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(
                                v.VariantType,
                                "subVariant",
                                StringComparison.OrdinalIgnoreCase));

                    if (!exists)
                    {
                        result.Errors.Add(
                            $"Feature '{feature.FeatureCode}' " +
                            $"references unknown concrete variant " +
                            $"'{variantName}'.");
                    }
                }
            }
        }

        // ================================================================
        // SPECIFICATIONS
        // ================================================================

        private static void ValidateSpecifications(
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            foreach (var specification in data.Specifications)
            {
                if (specification.SpecificationId <= 0)
                {
                    result.Errors.Add(
                        $"Specification '{specification.SpecificationCode}' " +
                        "has invalid specificationId.");
                }

                if (string.IsNullOrWhiteSpace(
                        specification.SpecificationCode))
                {
                    result.Errors.Add(
                        $"SpecificationId " +
                        $"{specification.SpecificationId} " +
                        "has no specificationCode.");
                }

                // Every specification is variant-level.
                // AppliesToVariants is therefore mandatory.

                if (specification.AppliesToVariants == null ||
                    specification.AppliesToVariants.Count == 0)
                {
                    result.Errors.Add(
                        $"Specification " +
                        $"'{specification.SpecificationCode}' " +
                        "has no appliesToVariants.");
                }
                else
                {
                    foreach (var variantName
                             in specification.AppliesToVariants
                                 .Where(x =>
                                     !string.IsNullOrWhiteSpace(x))
                                 .Distinct(
                                     StringComparer.OrdinalIgnoreCase))
                    {
                        var exists =
                            data.Variants.Any(v =>
                                string.Equals(
                                    v.VariantName,
                                    variantName,
                                    StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(
                                    v.VariantType,
                                    "subVariant",
                                    StringComparison.OrdinalIgnoreCase));

                        if (!exists)
                        {
                            result.Errors.Add(
                                $"Specification " +
                                $"'{specification.SpecificationCode}' " +
                                $"references unknown concrete variant " +
                                $"'{variantName}'.");
                        }
                    }
                }

                // Exactly one value type should be populated.

                var valueCount = 0;

                if (specification.NumericValue.HasValue)
                    valueCount++;

                if (!string.IsNullOrWhiteSpace(
                        specification.TextValue))
                {
                    valueCount++;
                }

                if (specification.BooleanValue.HasValue)
                    valueCount++;

                if (valueCount == 0)
                {
                    result.Warnings.Add(
                        $"Specification " +
                        $"'{specification.SpecificationCode}' " +
                        "has no numeric, text or boolean value.");
                }

                if (valueCount > 1)
                {
                    result.Errors.Add(
                        $"Specification " +
                        $"'{specification.SpecificationCode}' " +
                        "contains multiple value types.");
                }
            }
        }

        // ================================================================
        // FUEL EFFICIENCY
        // ================================================================

        private static void ValidateFuelEfficiency(
            BrochureExtractionDto data,
            BrochureImportValidationResult result)
        {
            foreach (var fuel in data.FuelEfficiencies)
            {
                if (!fuel.FuelEfficiency.HasValue)
                {
                    result.Warnings.Add(
                        "Fuel efficiency entry has no value.");
                }

                if (fuel.AppliesToVariants == null)
                {
                    result.Errors.Add(
                        "Fuel efficiency entry has null appliesToVariants.");

                    continue;
                }

                if (fuel.AppliesToVariants.Count == 0)
                {
                    result.Warnings.Add(
                        "Fuel efficiency entry has no appliesToVariants.");
                }

                foreach (var variantName
                         in fuel.AppliesToVariants
                             .Where(x =>
                                 !string.IsNullOrWhiteSpace(x))
                             .Distinct(
                                 StringComparer.OrdinalIgnoreCase))
                {
                    var exists =
                        data.Variants.Any(v =>
                            string.Equals(
                                v.VariantName,
                                variantName,
                                StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(
                                v.VariantType,
                                "subVariant",
                                StringComparison.OrdinalIgnoreCase));

                    if (!exists)
                    {
                        result.Errors.Add(
                            $"Fuel efficiency references " +
                            $"unknown concrete variant " +
                            $"'{variantName}'.");
                    }
                }
            }
        }

        // ================================================================
        // PRODUCTION CATALOG VALIDATION
        // ================================================================

        private async Task ValidateCatalogReferencesAsync(
            BrochureExtractionDto data,
            BrochureImportValidationResult result,
            CancellationToken cancellationToken)
        {
            // ------------------------------------------------------------
            // FEATURES
            // ------------------------------------------------------------

            var featureIds =
                data.Features
                    .Select(x => x.FeatureId)
                    .Where(x => x > 0)
                    .Distinct()
                    .ToList();

            if (featureIds.Count > 0)
            {
                var catalogFeatures =
                    await carsDbContext.Features
                        .Where(x =>
                            featureIds.Contains(
                                x.FeatureId))
                        .Select(x => new
                        {
                            x.FeatureId,
                            x.FeatureCode
                        })
                        .ToListAsync(
                            cancellationToken);

                var featureMap =
                    catalogFeatures.ToDictionary(
                        x => x.FeatureId);

                foreach (var id in featureIds)
                {
                    if (!featureMap.ContainsKey(id))
                    {
                        result.Errors.Add(
                            $"FeatureId {id} does not exist " +
                            "in Feature catalog.");
                    }
                }

                foreach (var feature in data.Features)
                {
                    if (!featureMap.TryGetValue(
                            feature.FeatureId,
                            out var catalogFeature))
                    {
                        continue;
                    }

                    if (!string.Equals(
                            feature.FeatureCode?.Trim(),
                            catalogFeature.FeatureCode?.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        result.Errors.Add(
                            $"FeatureId {feature.FeatureId} has " +
                            $"FeatureCode '{feature.FeatureCode}', " +
                            $"but catalog contains " +
                            $"'{catalogFeature.FeatureCode}'.");
                    }
                }
            }

            // ------------------------------------------------------------
            // FEATURE VALUE OPTIONS
            // ------------------------------------------------------------

            var featureOptionPairs =
                data.Features
                    .SelectMany(feature =>
                        (feature.ValueOptionIds ?? [])
                            .Distinct()
                            .Select(optionId => new
                            {
                                feature.FeatureId,
                                OptionId = optionId
                            }))
                    .Distinct()
                    .ToList();

            var valueOptionIds =
                featureOptionPairs
                    .Select(x => x.OptionId)
                    .Distinct()
                    .ToList();

            if (valueOptionIds.Count > 0)
            {
                var catalogOptions =
                    await carsDbContext.FeatureValueOptions
                        .Where(x =>
                            valueOptionIds.Contains(
                                x.FeatureValueOptionId))
                        .Select(x => new
                        {
                            x.FeatureValueOptionId,
                            x.FeatureId
                        })
                        .ToListAsync(
                            cancellationToken);

                var optionMap =
                    catalogOptions.ToDictionary(
                        x => x.FeatureValueOptionId);

                foreach (var optionId in valueOptionIds)
                {
                    if (!optionMap.ContainsKey(optionId))
                    {
                        result.Errors.Add(
                            $"FeatureValueOptionId {optionId} " +
                            "does not exist in catalog.");
                    }
                }

                foreach (var pair in featureOptionPairs)
                {
                    if (!optionMap.TryGetValue(
                            pair.OptionId,
                            out var catalogOption))
                    {
                        continue;
                    }

                    if (catalogOption.FeatureId != pair.FeatureId)
                    {
                        result.Errors.Add(
                            $"FeatureValueOptionId {pair.OptionId} " +
                            $"belongs to FeatureId " +
                            $"{catalogOption.FeatureId}, but was supplied " +
                            $"for FeatureId {pair.FeatureId}.");
                    }
                }
            }

            // ------------------------------------------------------------
            // SPECIFICATIONS
            // ------------------------------------------------------------

            var specificationIds =
                data.Specifications
                    .Select(x =>
                        x.SpecificationId)
                    .Where(x => x > 0)
                    .Distinct()
                    .ToList();

            if (specificationIds.Count > 0)
            {
                var catalogSpecifications =
                    await carsDbContext.Specifications
                        .Where(x =>
                            specificationIds.Contains(
                                x.SpecificationId))
                        .Select(x => new
                        {
                            x.SpecificationId,
                            x.SpecificationCode
                        })
                        .ToListAsync(
                            cancellationToken);

                var specificationMap =
                    catalogSpecifications.ToDictionary(
                        x => x.SpecificationId);

                foreach (var id in specificationIds)
                {
                    if (!specificationMap.ContainsKey(id))
                    {
                        result.Errors.Add(
                            $"SpecificationId {id} does not exist " +
                            "in Specification catalog.");
                    }
                }

                foreach (var specification
                         in data.Specifications)
                {
                    if (!specificationMap.TryGetValue(
                            specification.SpecificationId,
                            out var catalogSpecification))
                    {
                        continue;
                    }

                    if (!string.Equals(
                            specification.SpecificationCode?.Trim(),
                            catalogSpecification.SpecificationCode?.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        result.Errors.Add(
                            $"SpecificationId " +
                            $"{specification.SpecificationId} has " +
                            $"SpecificationCode " +
                            $"'{specification.SpecificationCode}', " +
                            $"but catalog contains " +
                            $"'{catalogSpecification.SpecificationCode}'.");
                    }
                }
            }

            // ------------------------------------------------------------
            // FUEL TYPES
            // ------------------------------------------------------------

            var fuelTypeIds =
                data.Engines
                    .SelectMany(x =>
                        x.EnginePerformances ?? [])
                    .Where(x =>
                        x.FuelTypeId.HasValue)
                    .Select(x =>
                        x.FuelTypeId!.Value)
                    .Distinct()
                    .ToList();

            if (fuelTypeIds.Count > 0)
            {
                var existingFuelTypeIds =
                    await carsDbContext.FuelTypes
                        .Where(x =>
                            fuelTypeIds.Contains(
                                x.FuelTypeId))
                        .Select(x =>
                            x.FuelTypeId)
                        .ToListAsync(
                            cancellationToken);

                var existingSet =
                    existingFuelTypeIds.ToHashSet();

                foreach (var id in fuelTypeIds)
                {
                    if (!existingSet.Contains(id))
                    {
                        result.Errors.Add(
                            $"FuelTypeId {id} does not exist " +
                            "in FuelType catalog.");
                    }
                }
            }
        }
    }


    // =====================================================================
    // VALIDATION RESULT
    // =====================================================================

    public sealed class BrochureImportValidationResult
    {
        public bool IsValid =>
            Errors.Count == 0;

        public List<string> Errors { get; } = [];

        public List<string> Warnings { get; } = [];
    }


    // =====================================================================
    // VALIDATION EXCEPTION
    // =====================================================================

    public sealed class BrochureImportValidationException
        : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public BrochureImportValidationException(
            IEnumerable<string> errors)
            : base(
                "Brochure import validation failed.")
        {
            Errors =
                errors.ToList();
        }
    }


    // =====================================================================
    // GENERAL IMPORT EXCEPTION
    // =====================================================================

    public sealed class BrochureImportException
        : Exception
    {
        public BrochureImportException(
            string message)
            : base(message)
        {
        }

        public BrochureImportException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }
    }
}