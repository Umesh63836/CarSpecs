namespace CarSpecAPI.Services.OpenAIServices
{
    public interface ICarBrochureAiService
    {
        Task<string> ProcessBrochurePdfAsync(byte[] pdfBytes, string fileName, IReadOnlyCollection<int> originalPageNumbers, CancellationToken cancellationToken = default);
    }
}