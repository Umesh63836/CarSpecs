using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;

namespace CarSpecAPI.Services
{
    public interface IBrochureImportService
    {
        Task<BrochureUploadResponse> UploadAsync(BrochureUploadRequest request, CancellationToken cancellationToken = default);
    }
}