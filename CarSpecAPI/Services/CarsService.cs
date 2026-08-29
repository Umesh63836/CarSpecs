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
                                .Include(v => v.Engine)
                                    .ThenInclude(e => e.FuelType)
                                .Include(v => v.Transmission)
                                .Include(v => v.Drivetrain)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Brand))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Model.Brand.BrandName.Contains(request.Brand));
            }

            if (!string.IsNullOrWhiteSpace(request.Model))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Model.ModelName.Contains(request.Model));
            }

            if (request.Displacement.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.Displacement == request.Displacement.Value);
            }

            if (request.MinPower.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.MaxPower >= request.MinPower.Value);
            }

            if (request.MaxPower.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.MaxPower <= request.MaxPower.Value);
            }

            if (request.MinTorque.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.MaxTorque >= request.MinTorque.Value);
            }

            if (request.MaxTorque.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.MaxTorque <= request.MaxTorque.Value);
            }

            if (request.IsTurbocharged.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.IsTurbocharged == request.IsTurbocharged.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.EmissionStandard))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.EmissionStandard.Contains(request.EmissionStandard));
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
                    v.Engine.FuelType.FuelType1.Contains(request.FuelType));
            }

            if (request.MinPrice.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.ExShowroomPrice >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.ExShowroomPrice <= request.MaxPrice.Value);
            }


            var models = await variantsQuery.GroupBy(v => new
            {
                modelId = v.ModelId,
                //brand = v.Model.Brand.BrandName,
                model = v.Model.ModelName,
                ImageUrl = v.Model.ModelImageUrl
            }).Select(g => new CarModelSearchResponseAiDto
            {
                ModelId = g.Key.modelId,
                Brand = g.Select(g => g.Model.Brand.BrandName).First(),
                Model = g.Key.model,
                ModelImageUrl = g.Key.ImageUrl,
                Variants = g.Select(v => new CarVariantSearchResponseAiDto
                {
                    VariantId = v.VariantId,
                    VariantName = v.VariantName,
                    ExShowroomPrice = v.ExShowroomPrice,
                    Displacement = v.Engine.Displacement,
                    MaxPower = v.Engine.MaxPower,
                    MaxTorque = v.Engine.MaxTorque,
                    IsTurbocharged = v.Engine.IsTurbocharged,
                    EmissionStandard = v.Engine.EmissionStandard,
                    FuelType = v.Engine.FuelType.FuelType1,
                    TransmissionType = v.Transmission.TransmissionType,
                    NumberOfGears = v.Transmission.NumberOfGears,
                    DrivetrainType = v.Drivetrain.DrivetrainType
                }).ToList()
            })
            .ToListAsync();

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
                                .Include(v => v.Engine)
                                    .ThenInclude(e => e.FuelType)
                                .Include(v => v.Transmission)
                                .Include(v => v.Drivetrain)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Brand))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Model.Brand.BrandName.Contains(request.Brand));
            }

            if (!string.IsNullOrWhiteSpace(request.Model))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Model.ModelName.Contains(request.Model));
            }

            if (request.Displacement.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.Displacement == request.Displacement.Value);
            }

            if (request.MinPower.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.MaxPower >= request.MinPower.Value);
            }

            if (request.MaxPower.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.MaxPower <= request.MaxPower.Value);
            }

            if (request.MinTorque.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.MaxTorque >= request.MinTorque.Value);
            }

            if (request.MaxTorque.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.MaxTorque <= request.MaxTorque.Value);
            }

            if (request.IsTurbocharged.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.IsTurbocharged == request.IsTurbocharged.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.EmissionStandard))
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.Engine.EmissionStandard.Contains(request.EmissionStandard));
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
                    v.Engine.FuelType.FuelType1.Contains(request.FuelType));
            }

            if (request.MinPrice.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.ExShowroomPrice >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                variantsQuery = variantsQuery.Where(v =>
                    v.ExShowroomPrice <= request.MaxPrice.Value);
            }


            var models = await variantsQuery.GroupBy(v => new
            {
                modelId = v.ModelId,
                //brand = v.Model.Brand.BrandName,
                model = v.Model.ModelName,
                ImageUrl = v.Model.ModelImageUrl
            }).Select(g => new CarModelSearchResponseDto
            {
                ModelId = g.Key.modelId,
                Brand = g.Select(g => g.Model.Brand.BrandName).First(),
                Model = g.Key.model,
                ModelImageUrl = g.Key.ImageUrl,
                Variants = g.Select(v => new CarVariantSearchResponseDto
                {
                    VariantId = v.VariantId,
                    VariantName = v.VariantName,
                    ExShowroomPrice = v.ExShowroomPrice,
                    MaxPower = v.Engine.MaxPower,
                    FuelType = v.Engine.FuelType.FuelType1,
                    TransmissionType = v.Transmission.TransmissionType,
                    DrivetrainType = v.Drivetrain.DrivetrainType
                }).ToList()
            })
            .ToListAsync();

            return new CarSearchResponseDto
            {
                TotalModels = models.Count,
                TotalVariants = models.Sum(x => x.Variants.Count),
                Models = models
            };
        }

    }
}
