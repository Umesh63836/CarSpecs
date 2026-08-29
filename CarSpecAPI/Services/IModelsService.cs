using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;

namespace CarSpecAPI.Services
{
    public interface IModelsService
    {
        Task<ModelDto?> CreateModelAsync(CreateModelDto dto);
        Task<ModelResponseDto> GetModelByIdAsync(int modelId);
        public Task<List<ModelDto>> GetModelsAsync(int brandId);
    }
}