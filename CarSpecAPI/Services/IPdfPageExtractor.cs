namespace CarSpecAPI.Services
{
    public interface IPdfPageExtractor
    {
        Task<byte[]> CreateSelectedPagesPdfAsync(Stream sourcePdf, IReadOnlyCollection<int> selectedPages, CancellationToken cancellationToken = default);
    }
}