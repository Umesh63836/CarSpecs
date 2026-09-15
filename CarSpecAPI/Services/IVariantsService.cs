using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Data.Models.ServiceModel;

namespace CarSpecAPI.Services
{
    public interface IVariantsService
    {
        Task<VariantDto?> CreateVariantAsync(int modelId, CreateVariantDto dto);
        Task<List<VariantDto>> GetVariantsAsync(int id);
        Task<VariantDetails?> GetVariantDetailsAsync(int variantId);
    }
}