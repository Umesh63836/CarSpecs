namespace CarSpecAPI.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string storagePath, string contentType, CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default);
    }
}