using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;

namespace CarSpecAPI.Services
{
    public interface IBrandsService
    {
        public Task<BrandDto> CreateBrandAsync(CreateBrandDto dto);
        public Task<List<BrandDto>> GetAllBrandsAsync();
    }
}