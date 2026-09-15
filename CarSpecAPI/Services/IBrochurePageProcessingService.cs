namespace CarSpecAPI.Services
{
    public interface IBrochurePageProcessingService
    {
        Task ProcessSelectedPagesAsync(int importDocumentId, string fileName, List<int> selectedPages, CancellationToken cancellationToken = default);
    }
}