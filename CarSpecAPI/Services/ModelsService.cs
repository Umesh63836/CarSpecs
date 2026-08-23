using CarSpecAPI.Data;
using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Services
{
    public class ModelsService : IModelsService
    {
        private readonly CarsDbContext carDbContext;

        public ModelsService(CarsDbContext carDbContext)
        {
            this.carDbContext = carDbContext;
        }

        public async Task<List<ModelDto>> GetModelsAsync(int brandId)
        {
            var result = await carDbContext.Models.Where(m => m.BrandId == brandId).Select(m => new ModelDto
            {
                ModelId = m.ModelId,
                ModelImageUrl = m.ModelImageUrl,
                ModelName = m.ModelName,
                MaxPower = m.Variants.Max(v => v.Engine.MaxPower),
                MinPower = m.Variants.Min(v => v.Engine.MaxPower),
                MinPrice = m.Variants
                .Min(v => v.ExShowroomPrice),

                MaxPrice = m.Variants
                .Max(v => v.ExShowroomPrice),
            }).ToListAsync();
            return result;
        }

        public async Task<VariantModelResponseDto> GetModelByIdAsync(int modelId)
        {
            var result = await carDbContext.Models.Where(m => m.ModelId == modelId).Include(m => m.Brand).Include(m => m.Variants).FirstAsync();
            VariantModelResponseDto dto = new VariantModelResponseDto
            {
                ModelId = result.ModelId,
                BrandName = result.Brand.BrandName,
                ModelImageUrl = result.ModelImageUrl,
                ModelName = result.ModelName,
                MinPrice = result.Variants
                .Min(v => v.ExShowroomPrice),

                MaxPrice = result.Variants
                .Max(v => v.ExShowroomPrice),
            };
            return dto;
        }


        public async Task<ModelDto?> CreateModelAsync(CreateModelDto dto)
        {
            var brandExists = await carDbContext.Brands
                .AnyAsync(x => x.BrandId == dto.BrandId);

            if (!brandExists)
                return null;

            var model = new Model
            {
                ModelName = dto.ModelName,
                BrandId = dto.BrandId,
                IsActive = dto.IsActive,
                LaunchYear = dto.LaunchYear,
                DiscontinuedYear = dto.DiscontinuedYear,
                ModelImageUrl = dto.ModelImageUrl
            };

            carDbContext.Models.Add(model);
            await carDbContext.SaveChangesAsync();

            return new ModelDto
            {
                ModelId = model.ModelId,
                ModelName = model.ModelName,
                ModelImageUrl = model.ModelImageUrl
            };
        }
    }
}
