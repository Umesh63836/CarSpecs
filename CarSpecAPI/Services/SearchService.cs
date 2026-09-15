using CarSpecAPI.Data;
using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Services
{
    public class SearchService : ISearchService
    {
        private readonly CarsDbContext carsDbContext;

        public SearchService(CarsDbContext carsDbContext)
        {
            this.carsDbContext = carsDbContext;
        }

        public async Task<List<SearchResultDto>> SearchAsync(string searchParameter)
        {
            if (string.IsNullOrWhiteSpace(searchParameter))
                return new List<SearchResultDto>();

            searchParameter = searchParameter.Trim();

            var searchModel = await carsDbContext.Models
                .Where(m => m.ModelName.Contains(searchParameter) || m.Brand.BrandName.Contains(searchParameter))
                .Select(m => new SearchResultDto
                {
                    ResultType = "Model",
                    Id = m.ModelId,
                    Name = m.ModelName,
                    BrandName = m.Brand.BrandName,
                    ModelName = m.ModelName
                })
                .ToListAsync();

            var variants = await carsDbContext.Variants
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .Include(v => v.Powertrain)
                    .ThenInclude(p => p.Engine)
                .ToListAsync();

            var variantResults = variants
                .Where(v => v.VariantName.Contains(searchParameter) ||
                    (v.Powertrain.Engine != null && v.Powertrain.Engine.EngineName.Contains(searchParameter)))
                .Select(v => new SearchResultDto
                {
                    ResultType = "Variant",
                    Id = v.VariantId,
                    Name = v.VariantName + " " + (v.Powertrain.Engine?.EngineName ?? string.Empty),
                    BrandName = v.Model.Brand.BrandName,
                    ModelName = v.Model.ModelName,
                })
                .ToList();

            return searchModel.Concat(variantResults).ToList();
        }
    }
}
