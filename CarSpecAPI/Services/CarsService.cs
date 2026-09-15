using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Services
{
    public class CarsService : ICarsService
    {
        private readonly CarsDbContext context;

        public CarsService(CarsDbContext context)
        {
            this.context = context;
        }

        public async Task<CarSearchResponseAiDto> SearchCarsAiAsync(CarsSearchRequest request)
        {
            var variantsQuery = context.Variants
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .Include(v => v.Powertrain)
                    .ThenInclude(p => p.Engine)
                        .ThenInclude(e => e.EnginePerformances)
                            .ThenInclude(ep => ep.FuelType)
                .Include(v => v.Transmission)
                .Include(v => v.Drivetrain)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Brand))
            {
                variantsQuery = variantsQuery.Where(v => v.Model.Brand.BrandName.Contains(request.Brand));
            }

            if (!string.IsNullOrWhiteSpace(request.Model))
            {
                variantsQuery = variantsQuery.Where(v => v.Model.ModelName.Contains(request.Model));
            }

            if (request.Displacement.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null && v.Powertrain.Engine.Displacement == request.Displacement.Value);
            }

            if (request.MinPower.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null &&
                    v.Powertrain.Engine.EnginePerformances.Any(ep => ep.MaxPower.HasValue) &&
                    v.Powertrain.Engine.EnginePerformances.Where(ep => ep.MaxPower.HasValue).Min(ep => ep.MaxPower!.Value) >= request.MinPower.Value);
            }

            if (request.MaxPower.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null &&
                    v.Powertrain.Engine.EnginePerformances.Any(ep => ep.MaxPower.HasValue) &&
                    v.Powertrain.Engine.EnginePerformances.Where(ep => ep.MaxPower.HasValue).Max(ep => ep.MaxPower!.Value) <= request.MaxPower.Value);
            }

            if (request.MinTorque.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null &&
                    v.Powertrain.Engine.EnginePerformances.Any(ep => ep.MaxTorque.HasValue) &&
                    v.Powertrain.Engine.EnginePerformances.Where(ep => ep.MaxTorque.HasValue).Min(ep => ep.MaxTorque!.Value) >= request.MinTorque.Value);
            }

            if (request.MaxTorque.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null &&
                    v.Powertrain.Engine.EnginePerformances.Any(ep => ep.MaxTorque.HasValue) &&
                    v.Powertrain.Engine.EnginePerformances.Where(ep => ep.MaxTorque.HasValue).Max(ep => ep.MaxTorque!.Value) <= request.MaxTorque.Value);
            }

            if (request.IsTurbocharged.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null && v.Powertrain.Engine.IsTurbocharged == request.IsTurbocharged.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.EmissionStandard))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null && v.Powertrain.Engine.EmissionStandard != null &&
                    v.Powertrain.Engine.EmissionStandard.Contains(request.EmissionStandard));
            }

            if (!string.IsNullOrWhiteSpace(request.TransmissionType))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Transmission.TransmissionType.Contains(request.TransmissionType));
            }

            if (request.NumberOfGears.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Transmission.NumberOfGears == request.NumberOfGears.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.DrivetrainType))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Drivetrain.DrivetrainType.Contains(request.DrivetrainType));
            }

            if (!string.IsNullOrWhiteSpace(request.FuelType))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null &&
                    v.Powertrain.Engine.EnginePerformances.Any(ep => ep.FuelType != null && ep.FuelType.FuelType1.Contains(request.FuelType)));
            }

            if (request.MinPrice.HasValue)
            {
                variantsQuery = variantsQuery.Where(v => v.ExShowroomPrice >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                variantsQuery = variantsQuery.Where(v => v.ExShowroomPrice <= request.MaxPrice.Value);
            }

            var models = (await variantsQuery.ToListAsync())
                .GroupBy(v => new
                {
                    modelId = v.ModelId,
                    model = v.Model.ModelName,
                    ImageUrl = v.Model.ModelImageUrl
                })
                .Select(g => new CarModelSearchResponseAiDto
                {
                    ModelId = g.Key.modelId,
                    Brand = g.Select(g => g.Model.Brand.BrandName).First(),
                    Model = g.Key.model,
                    ModelImageUrl = g.Key.ImageUrl,
                    Variants = g.Select(v =>
                    {
                        var engine = v.Powertrain.Engine;
                        var maxPower = engine?.EnginePerformances
                            .Where(ep => ep.MaxPower.HasValue)
                            .Select(ep => ep.MaxPower)
                            .Max();
                        var maxTorque = engine?.EnginePerformances
                            .Where(ep => ep.MaxTorque.HasValue)
                            .Select(ep => ep.MaxTorque)
                            .Max();
                        var performance = engine?.EnginePerformances
                            .Where(ep => ep.MaxPower.HasValue)
                            .OrderByDescending(ep => ep.MaxPower)
                            .FirstOrDefault();

                        return new CarVariantSearchResponseAiDto
                        {
                            VariantId = v.VariantId,
                            VariantName = v.VariantName,
                            ExShowroomPrice = v.ExShowroomPrice,
                            Displacement = engine?.Displacement,
                            MaxPower = maxPower,
                            MaxTorque = maxTorque,
                            IsTurbocharged = engine?.IsTurbocharged,
                            EmissionStandard = engine?.EmissionStandard,
                            FuelType = performance?.FuelType?.FuelType1 ?? string.Empty,
                            TransmissionType = v.Transmission.TransmissionType,
                            NumberOfGears = v.Transmission.NumberOfGears,
                            DrivetrainType = v.Drivetrain.DrivetrainType
                        };
                    }).ToList()
                })
                .ToList();

            return new CarSearchResponseAiDto
            {
                TotalModels = models.Count,
                TotalVariants = models.Sum(x => x.Variants.Count),
                Models = models
            };
        }

        public async Task<CarSearchResponseDto> SearchCarsAsync(CarsSearchRequest request)
        {
            var variantsQuery = context.Variants
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .Include(v => v.Powertrain)
                    .ThenInclude(p => p.Engine)
                        .ThenInclude(e => e.EnginePerformances)
                            .ThenInclude(ep => ep.FuelType)
                .Include(v => v.Transmission)
                .Include(v => v.Drivetrain)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Brand))
            {
                variantsQuery = variantsQuery.Where(v => v.Model.Brand.BrandName.Contains(request.Brand));
            }

            if (!string.IsNullOrWhiteSpace(request.Model))
            {
                variantsQuery = variantsQuery.Where(v => v.Model.ModelName.Contains(request.Model));
            }

            if (request.Displacement.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null && v.Powertrain.Engine.Displacement == request.Displacement.Value);
            }

            if (request.MinPower.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null &&
                    v.Powertrain.Engine.EnginePerformances.Any(ep => ep.MaxPower.HasValue) &&
                    v.Powertrain.Engine.EnginePerformances.Where(ep => ep.MaxPower.HasValue).Min(ep => ep.MaxPower!.Value) >= request.MinPower.Value);
            }

            if (request.MaxPower.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null &&
                    v.Powertrain.Engine.EnginePerformances.Any(ep => ep.MaxPower.HasValue) &&
                    v.Powertrain.Engine.EnginePerformances.Where(ep => ep.MaxPower.HasValue).Max(ep => ep.MaxPower!.Value) <= request.MaxPower.Value);
            }

            if (request.MinTorque.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null &&
                    v.Powertrain.Engine.EnginePerformances.Any(ep => ep.MaxTorque.HasValue) &&
                    v.Powertrain.Engine.EnginePerformances.Where(ep => ep.MaxTorque.HasValue).Min(ep => ep.MaxTorque!.Value) >= request.MinTorque.Value);
            }

            if (request.MaxTorque.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null &&
                    v.Powertrain.Engine.EnginePerformances.Any(ep => ep.MaxTorque.HasValue) &&
                    v.Powertrain.Engine.EnginePerformances.Where(ep => ep.MaxTorque.HasValue).Max(ep => ep.MaxTorque!.Value) <= request.MaxTorque.Value);
            }

            if (request.IsTurbocharged.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null && v.Powertrain.Engine.IsTurbocharged == request.IsTurbocharged.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.EmissionStandard))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null && v.Powertrain.Engine.EmissionStandard != null &&
                    v.Powertrain.Engine.EmissionStandard.Contains(request.EmissionStandard));
            }

            if (!string.IsNullOrWhiteSpace(request.TransmissionType))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Transmission.TransmissionType.Contains(request.TransmissionType));
            }

            if (request.NumberOfGears.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Transmission.NumberOfGears == request.NumberOfGears.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.DrivetrainType))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Drivetrain.DrivetrainType.Contains(request.DrivetrainType));
            }

            if (!string.IsNullOrWhiteSpace(request.FuelType))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Powertrain.Engine != null &&
                    v.Powertrain.Engine.EnginePerformances.Any(ep => ep.FuelType != null && ep.FuelType.FuelType1.Contains(request.FuelType)));
            }

            if (request.MinPrice.HasValue)
            {
                variantsQuery = variantsQuery.Where(v => v.ExShowroomPrice >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                variantsQuery = variantsQuery.Where(v => v.ExShowroomPrice <= request.MaxPrice.Value);
            }

            var models = (await variantsQuery.ToListAsync())
                .GroupBy(v => new
                {
                    modelId = v.ModelId,
                    model = v.Model.ModelName,
                    ImageUrl = v.Model.ModelImageUrl
                })
                .Select(g => new CarModelSearchResponseDto
                {
                    ModelId = g.Key.modelId,
                    Brand = g.Select(g => g.Model.Brand.BrandName).First(),
                    Model = g.Key.model,
                    ModelImageUrl = g.Key.ImageUrl,
                    Variants = g.Select(v =>
                    {
                        var engine = v.Powertrain.Engine;
                        var maxPower = engine?.EnginePerformances
                            .Where(ep => ep.MaxPower.HasValue)
                            .Select(ep => ep.MaxPower)
                            .Max();

                        return new CarVariantSearchResponseDto
                        {
                            VariantId = v.VariantId,
                            VariantName = v.VariantName,
                            ExShowroomPrice = v.ExShowroomPrice,
                            MaxPower = maxPower,
                            FuelType = engine?.EnginePerformances
                                .Where(ep => ep.MaxPower.HasValue)
                                .OrderByDescending(ep => ep.MaxPower)
                                .Select(ep => ep.FuelType != null ? ep.FuelType.FuelType1 : string.Empty)
                                .FirstOrDefault() ?? string.Empty,
                            TransmissionType = v.Transmission.TransmissionType,
                            DrivetrainType = v.Drivetrain.DrivetrainType
                        };
                    }).ToList()
                })
                .ToList();

            return new CarSearchResponseDto
            {
                TotalModels = models.Count,
                TotalVariants = models.Sum(x => x.Variants.Count),
                Models = models
            };
        }
    }
}
