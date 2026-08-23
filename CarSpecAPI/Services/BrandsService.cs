using CarSpecAPI.Data;
using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CarSpecAPI.Services
{
    public class BrandsService : IBrandsService
    {
        private CarsDbContext carsDbContext;
        private IMemoryCache cache;

        public BrandsService(CarsDbContext carsDbContext, IMemoryCache cache) 
        { 
            this.carsDbContext = carsDbContext;
            this.cache = cache;
        }

        public async Task<List<BrandDto>> GetAllBrandsAsync()
        {
            if (cache.TryGetValue("brands", out List<BrandDto>? brands))
                return brands;

            var result = await carsDbContext.Brands.Select(brand => new BrandDto
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                LogoUrl = brand.LogoUrl
            }).ToListAsync();

            cache.Set("brands", result, TimeSpan.FromMinutes(10));

            //var brands = await carsDbContext.Brands.ToListAsync();
            //List<BrandDto> result = new List<BrandDto>();
            //foreach (var brand in brands)
            //{
            //    BrandDto brandDto = new BrandDto();
            //    brandDto.BrandId = brand.BrandId;
            //    brandDto.BrandName = brand.BrandName;
            //    brandDto.LogoURL = brand.LogoUrl;
            //    result.Add(brandDto);
            //}
            return result;
        }

        public async Task<BrandDto> CreateBrandAsync(CreateBrandDto dto)
        {
            var brand = new Brand
            {
                BrandName = dto.BrandName,
                LogoUrl = dto.LogoUrl,
            };
            await carsDbContext.AddAsync(brand);
            await carsDbContext.SaveChangesAsync();
            cache.Remove("brands");
            return new BrandDto
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                LogoUrl = brand.LogoUrl
            };
        }

    }
}
