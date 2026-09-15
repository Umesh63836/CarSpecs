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
            var models = await carDbContext.Models
                .Where(m => m.BrandId == brandId)
                .Include(m => m.Variants)
                    .ThenInclude(v => v.Powertrain)
                        .ThenInclude(p => p.Engine)
                            .ThenInclude(e => e.EnginePerformances)
                                .ThenInclude(ep => ep.FuelType)
                .Include(m => m.Variants)
                    .ThenInclude(v => v.VariantFuelEfficiencies)
                .Include(m => m.ModelSafetyRatings)
                .ToListAsync();

            var result = models.Select(m =>
            {
                var variants = m.Variants;
                var powertrainEngines = variants
                    .Where(v => v.Powertrain?.Engine != null)
                    .Select(v => v.Powertrain!.Engine!)
                    .ToList();
                var enginePerformances = powertrainEngines
                    .SelectMany(e => e.EnginePerformances)
                    .ToList();

                var manufacturerEfficiency = variants
                    .SelectMany(v => v.VariantFuelEfficiencies)
                    .FirstOrDefault(f => f.SourceType == "Manufacturer");

                var araiEfficiency = variants
                    .SelectMany(v => v.VariantFuelEfficiencies)
                    .FirstOrDefault(f => f.SourceType == "ARAI");

                var selectedEfficiency = manufacturerEfficiency ?? araiEfficiency;

                var safetyRating = m.ModelSafetyRatings.Where(s => s.IsActive)
                    .OrderByDescending(s => s.Agency == "Bharat NCAP")
                    .ThenByDescending(s => s.TestYear)
                    .FirstOrDefault();

                return new ModelDto
                {
                    ModelId = m.ModelId,
                    ModelImageUrl = m.ModelImageUrl,
                    ModelName = m.ModelName,
                    MinPrice = variants.Select(v => v.ExShowroomPrice).Min(),
                    MaxPrice = variants.Select(v => v.ExShowroomPrice).Max(),
                    MinPower = enginePerformances.Where(ep => ep.MaxPower.HasValue).Select(ep => ep.MaxPower).Min(),
                    MaxPower = enginePerformances.Where(ep => ep.MaxPower.HasValue).Select(ep => ep.MaxPower).Max(),
                    SafetyRating = safetyRating?.Rating,
                    SafetyRatingSource = safetyRating?.Agency,
                    SafetyTestYear = safetyRating?.TestYear,
                    EngineCC = powertrainEngines.Where(e => e.Displacement.HasValue).Select(e => e.Displacement).Distinct().OrderBy(cc => cc).ToList(),
                    FuelTypes = enginePerformances.Where(ep => ep.FuelType != null).Select(ep => ep.FuelType!.FuelType1).Distinct().OrderBy(f => f).ToList(),
                    FuelEfficiency = selectedEfficiency?.FuelEfficiency,
                    FuelEfficiencyUnit = selectedEfficiency?.FuelEfficiencyUnit,
                    FuelEfficiencySource = selectedEfficiency?.SourceType
                };
            }).ToList();

            return result;
        }

        public async Task<ModelResponseDto> GetModelByIdAsync(int modelId)
        {
            var result = await carDbContext.Models.Where(m => m.ModelId == modelId).Include(m => m.Brand).Include(m => m.Variants).FirstAsync();
            ModelResponseDto dto = new ModelResponseDto
            {
                ModelId = result.ModelId,
                BrandName = result.Brand.BrandName,
                ModelImageUrl = result.ModelImageUrl,
                ModelName = result.ModelName,
                MinPrice = result.Variants.Min(v => v.ExShowroomPrice),
                MaxPrice = result.Variants.Max(v => v.ExShowroomPrice),
            };
            return dto;
        }

        public async Task<ModelDto?> CreateModelAsync(CreateModelDto dto)
        {
            var brandExists = await carDbContext.Brands.AnyAsync(x => x.BrandId == dto.BrandId);

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
