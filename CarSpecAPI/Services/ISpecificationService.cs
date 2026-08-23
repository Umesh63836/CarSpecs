using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;

namespace CarSpecAPI.Services
{
    public interface ISpecificationService
    {
        Task<DrivetrainDto> CreateDrivetrainAsync(CreateDrivetrainDto dto);
        Task<EngineDto?> CreateEngineAsync(CreateEngineDto dto);
        Task<TransmissionDto> CreateTransmissionAsync(CreateTransmissionDto dto);
        Task<List<SelectDrivetrainDto>> GetAllDrivetrainsAsync();
        Task<List<SelectEngineDto>> GetAllEnginesAsync();
        Task<List<SelectFuelTypeDto>> GetAllFuelTypesAsync();
        Task<List<SelectTransmissionDto>> GetAllTransmissionsAsync();
        public Task<VariantSpecsDto> GetVariantSpecsAsync(int variantId); 

    }
}