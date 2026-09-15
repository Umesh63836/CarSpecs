using CarSpecAPI.Data;
using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Data.Models.ServiceModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Services
{
    public class VariantsService : IVariantsService
    {
        private readonly CarsDbContext carsDbContext;

        public VariantsService(CarsDbContext carsDbContext)
        {
            this.carsDbContext = carsDbContext;
        }

        public async Task<List<VariantDto>> GetVariantsAsync(int modelId)
        {
            var variants = await carsDbContext.Variants
                .Where(v => v.ModelId == modelId)
                .Include(v => v.Powertrain)
                    .ThenInclude(p => p.Engine)
                        .ThenInclude(e => e.EnginePerformances)
                            .ThenInclude(ep => ep.FuelType)
                .Include(v => v.Transmission)
                .ToListAsync();

            return variants.Select(v =>
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

                return new VariantDto
                {
                    VariantId = v.VariantId,
                    VariantName = v.VariantName,
                    CubicCapacity = engine?.Displacement,
                    isTurbocharged = engine?.IsTurbocharged ?? false,
                    FuelType = performance?.FuelType?.FuelType1 ?? string.Empty,
                    TransmissionType = v.Transmission.TransmissionType,
                    MaxPower = maxPower,
                    MaxTorque = maxTorque,
                    ExShowroomPrice = v.ExShowroomPrice
                };
            }).ToList();
        }

        public async Task<VariantDto?> CreateVariantAsync(int modelId, CreateVariantDto dto)
        {
            var modelExists = await carsDbContext.Models.AnyAsync(x => x.ModelId == modelId);

            if (!modelExists)
                return null;

            var powertrainExists = await carsDbContext.Powertrains
                .AnyAsync(x => x.PowertrainId == dto.PowertrainId);

            if (!powertrainExists)
                return null;

            var variant = new Variant
            {
                ModelId = modelId,
                VariantName = dto.VariantName,
                PowertrainId = dto.PowertrainId,
                TransmissionId = dto.TransmissionId,
                DrivetrainId = dto.DrivetrainId,
                ExShowroomPrice = dto.ExShowroomPrice
            };

            carsDbContext.Variants.Add(variant);
            await carsDbContext.SaveChangesAsync();

            var variantImage = new VariantImage
            {
                VariantId = variant.VariantId,
                ImageUrl = dto.VariantImageUrl,
                IsPrimary = true
            };

            carsDbContext.VariantImages.Add(variantImage);
            await carsDbContext.SaveChangesAsync();

            var createdVariant = await carsDbContext.Variants
                .Include(x => x.Powertrain)
                    .ThenInclude(p => p.Engine)
                        .ThenInclude(e => e.EnginePerformances)
                            .ThenInclude(ep => ep.FuelType)
                .Include(x => x.Transmission)
                .FirstAsync(x => x.VariantId == variant.VariantId);

            var engine = createdVariant.Powertrain?.Engine;
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

            return new VariantDto
            {
                VariantId = createdVariant.VariantId,
                VariantName = createdVariant.VariantName,
                FuelType = performance?.FuelType?.FuelType1 ?? string.Empty,
                CubicCapacity = engine?.Displacement,
                isTurbocharged = engine?.IsTurbocharged ?? false,
                TransmissionType = createdVariant.Transmission.TransmissionType,
                MaxPower = maxPower,
                MaxTorque = maxTorque,
                ExShowroomPrice = createdVariant.ExShowroomPrice
            };
        }

        public async Task<VariantDetails> GetVariantDetailsAsync(int variantId)
        {
            var variant = await carsDbContext.Variants
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .Include(v => v.Powertrain)
                    .ThenInclude(p => p.Engine)
                        .ThenInclude(e => e.EnginePerformances)
                            .ThenInclude(ep => ep.FuelType)
                .FirstOrDefaultAsync(v => v.VariantId == variantId);

            if (variant == null)
                return new VariantDetails();

            var engine = variant.Powertrain?.Engine;
            var performance = engine?.EnginePerformances
                .Where(ep => ep.MaxPower.HasValue)
                .OrderByDescending(ep => ep.MaxPower)
                .FirstOrDefault();

            return new VariantDetails
            {
                BodyType = variant.Model.BodyType,
                VehicleCategory = variant.Model.Category,
                ExShowroomPrice = variant.ExShowroomPrice,
                SeatingCapacity = variant.SeatingCapacity,
                CubicCapacity = engine?.Displacement,
                FuelType = performance?.FuelType?.FuelType1 ?? string.Empty,
                KerbWeight = variant.KerbWeight
            };
        }
    }
}
