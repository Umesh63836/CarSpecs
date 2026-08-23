using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;

namespace CarSpecAPI.Services
{
    public interface IVariantsService
    {
        Task<VariantDto?> CreateVariantAsync(int modelId, CreateVariantDto dto);
        public Task<List<VariantDto>> GetVariantsAsync(int id);
    }
}