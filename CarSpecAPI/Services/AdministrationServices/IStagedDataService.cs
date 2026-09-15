using CarSpecAPI.Data.Models.DataExtractionModel;

namespace CarSpecAPI.Services.AdministrationServices
{
    public interface IStagedDataService
    {
        Task AttachFuelEfficiencyToVariantAsync(int importFuelEfficiencyId, int importVariantId, CancellationToken cancellationToken);
        Task<int> CreateDrivetrainAsync(CreateStagedDrivetrainDto dto, CancellationToken cancellationToken);
        Task<int> CreateFeatureAsync(CreateStagedFeatureDto dto, CancellationToken cancellationToken);
        Task<int> CreateFuelEfficiencyAsync(CreateStagedFuelEfficiencyDto dto, CancellationToken cancellationToken);
        Task<int> CreateFuelTypeAsync(CreateStagedFuelTypeDto dto, CancellationToken cancellationToken);
        Task<int> CreateModelDimensionsAsync(int importModelId, CreateStagedModelDimensionsDto dto, CancellationToken cancellationToken);
        Task<int> CreatePowertrainAsync(CreateStagedPowertrainDto dto, CancellationToken cancellationToken);
        Task<int> CreateEngineAsync(int importPowertrainId, CreateStagedEngineDto dto, CancellationToken cancellationToken);
        Task<int> CreateEnginePerformanceAsync(int importEngineId, CreateStagedEnginePerformanceDto dto, CancellationToken cancellationToken);
        Task<int> CreateMotorPerformanceAsync(int importPowertrainId, CreateStagedMotorPerformanceDto dto, CancellationToken cancellationToken);
        Task<int> CreateTransmissionAsync(CreateStagedTransmissionDto dto, CancellationToken cancellationToken);
        Task<int> CreateSpecificationAsync(CreateStagedSpecificationDto dto, CancellationToken cancellationToken);
        Task<int> CreateWarrantyAsync(CreateStagedWarrantyDto dto, CancellationToken cancellationToken);
        Task<StagedModelDetailDto> GetModelAsync(int importModelId, CancellationToken cancellationToken);
        Task<PaginatedStagedModelResponseDto> GetModelsAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken);
        Task UpdateDrivetrainAsync(int importDrivetrainId, UpdateStagedDrivetrainDto dto, CancellationToken cancellationToken);
        Task UpdateEngineAsync(int importEngineId, UpdateStagedEngineDto dto, CancellationToken cancellationToken);
        Task UpdateEnginePerformanceAsync(int importEnginePerformanceId, UpdateStagedEnginePerformanceDto dto, CancellationToken cancellationToken);
        Task UpdateFeatureVariantAsync(int importFeatureVariantId, UpdateStagedFeatureDto dto, CancellationToken cancellationToken);
        Task UpdateFuelEfficiencyAsync(int importFuelEfficiencyId, UpdateStagedFuelEfficiencyDto dto, CancellationToken cancellationToken);
        Task UpdateModelAsync(int importModelId, UpdateStagedModelDto dto, CancellationToken cancellationToken);
        Task UpdateMotorPerformanceAsync(int importMotorPerformanceId, UpdateStagedMotorPerformanceDto dto, CancellationToken cancellationToken);
        Task UpdatePowertrainAsync(int importPowertrainId, UpdateStagedPowertrainDto dto, CancellationToken cancellationToken);
        Task UpdateSpecificationVariantAsync(int importSpecificationVariantId, UpdateStagedSpecificationDto dto, CancellationToken cancellationToken);
        Task UpdateTransmissionAsync(int importTransmissionId, UpdateStagedTransmissionDto dto, CancellationToken cancellationToken);
        Task UpdateVariantAsync(int importVariantId, UpdateStagedVariantDto dto, CancellationToken cancellationToken);
        Task UpdateWarrantyAsync(int importWarrantyId, UpdateStagedWarrantyDto dto, CancellationToken cancellationToken);
        Task<StagedPowertrainDependenciesDto> GetPowertrainDependenciesAsync(CancellationToken cancellationToken = default);
        Task<StagedFeatureSpecificationCatalogDto> GetFeatureSpecificationCatalogAsync(CancellationToken cancellationToken = default);
        Task<List<StagedPowertrainLookupDto>>GetPowertrainsForModelAsync( int importModelId);
        Task<List<StagedTransmissionLookupDto>> GetTransmissionsForModelAsync(int importModelId);
        Task<List<StagedDrivetrainLookupDto>> GetDrivetrainsForModelAsync(int importModelId);
        Task<ReplaceStagedPowertrainReferenceResultDto> ReplaceVariantPowertrainReferenceAsync(int importVariantId, ReplaceStagedPowertrainReferenceDto dto);
        Task<ReplaceStagedTransmissionReferenceResultDto> ReplaceVariantTransmissionReferenceAsync(int importVariantId, ReplaceStagedTransmissionReferenceDto dto);
        Task<ReplaceStagedDrivetrainReferenceResultDto> ReplaceVariantDrivetrainReferenceAsync(int importVariantId, ReplaceStagedDrivetrainReferenceDto dto);

    }
}