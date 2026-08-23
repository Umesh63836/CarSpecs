using CarSpecAPI.Data;
using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;
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
            var result = await carsDbContext.Variants.Where(v => v.ModelId == modelId).Select(v => new VariantDto
            {
                VariantId = v.VariantId,
                VariantName = v.VariantName,
                CubicCapacity = v.Engine.Displacement,
                isTurbocharged = v.Engine.IsTurbocharged,
                FuelType = v.Engine.FuelType.FuelType1,
                TransmissionType = v.Transmission.TransmissionType,
                MaxPower = v.Engine.MaxPower,
                MaxTorque = v.Engine.MaxTorque,
                ExShowroomPrice = v.ExShowroomPrice
            }).ToListAsync();
            return result;            
        }

        public async Task<VariantDto?> CreateVariantAsync(int modelId, CreateVariantDto dto)
        {
            var modelExists = await carsDbContext.Models
                .AnyAsync(x => x.ModelId == modelId);

            if (!modelExists)
                return null;

            var variant = new Variant
            {
                ModelId = modelId,
                VariantName = dto.VariantName,
                EngineId = dto.EngineId,
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
            .Include(x => x.Engine)
            .ThenInclude(x => x.FuelType)
            .Include(x => x.Transmission)
            .Where(x => x.VariantId == variant.VariantId).FirstAsync();

            return new VariantDto
            {
                VariantId = createdVariant.VariantId,
                VariantName = createdVariant.VariantName,
                FuelType = createdVariant.Engine.FuelType.FuelType1,
                CubicCapacity = createdVariant.Engine.Displacement,
                isTurbocharged = createdVariant.Engine.IsTurbocharged,
                TransmissionType = createdVariant.Transmission.TransmissionType,
                MaxPower = createdVariant.Engine.MaxPower,
                MaxTorque = createdVariant.Engine.MaxTorque,
                ExShowroomPrice = createdVariant.ExShowroomPrice
            };
        }
    }
}
