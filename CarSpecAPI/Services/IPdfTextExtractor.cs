namespace CarSpecAPI.Services
{
    public interface IPdfTextExtractor
    {
        Task<string> ExtractTextAsync(Stream pdfStream, CancellationToken cancellationToken = default);
        Task<string> ExtractPageTextAsync(Stream pdfStream, int pageNumber, CancellationToken cancellationToken = default);
    }
}