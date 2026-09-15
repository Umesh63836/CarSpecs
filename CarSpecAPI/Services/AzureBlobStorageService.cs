using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using OpenAI.Containers;

namespace CarSpecAPI.Services
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobContainerClient container;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"] ?? throw new InvalidOperationException("Azure Storage connection string is missing.");
            var containerName = configuration["AzureStorage:BrochureContainer"] ?? "car-brochures";
            var serviceClient = new BlobServiceClient(connectionString);
            container = serviceClient.GetBlobContainerClient(containerName);
        }

        public async Task<string> UploadAsync(Stream fileStream, string storagePath, string contentType, CancellationToken cancellationToken = default)
        {
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var blobClient = container.GetBlobClient(storagePath);
            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };

            await blobClient.UploadAsync(fileStream, options, cancellationToken);

            return storagePath;
        }

        public async Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default)
        {
            var blobClient = container.GetBlobClient(path);
            var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

            return response.Value.Content;
        }
    }
}
