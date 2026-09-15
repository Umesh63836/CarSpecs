using CarSpecAPI.Data.Models.DataExtractionModel;

namespace CarsSpecAPI.Services
{
    public interface IImportBatchService
    {
        Task<PaginatedImportBatchResponseDto> GetImportBatchesAsync(int pageNumber, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<StageBatchResultDto> StageBatchAsync(int importBatchId, CancellationToken cancellationToken = default);
    }
}