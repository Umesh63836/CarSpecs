using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Entities;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CarSpecAPI.Services
{
    public class BrochureImportService : IBrochureImportService
    {
        private readonly CarsDbContext db;
        private readonly IFileStorageService storage;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IPdfTextExtractor pdfTextExtractor;

        public BrochureImportService(CarsDbContext db, IFileStorageService storage, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IPdfTextExtractor pdfTextExtractor)
        {
            this.db = db;
            this.storage = storage;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            this.pdfTextExtractor = pdfTextExtractor;
        }

        public async Task<BrochureUploadResponse> UploadAsync(BrochureUploadRequest request, CancellationToken cancellationToken = default)
        {
            if (request.File == null || request.File.Length == 0)
            {
                throw new ArgumentException("Brochure file is required.");
            }

            if (!string.Equals(request.File.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Only PDF brochures are currently supported.");
            }

            const long maxFileSize = 25 * 1024 * 1024;

            if (request.File.Length > maxFileSize)
            {
                throw new ArgumentException("Brochure cannot be larger than 25 MB.");
            }

            
            // 1. Create DataSource
            var dataSource = new DataSource
            {
                SourceName = request.SourceName,
                SourceType = "OfficialBrochure",
                SourceUrl = request.SourceUrl,
                Publisher = request.Publisher,
                PublishedDate = request.PublishedDate,
                RetrievedDate = DateTime.UtcNow,
            };
            db.DataSources.Add(dataSource);
            await db.SaveChangesAsync(cancellationToken);

            // 2. Create ImportBatch
            var adminId = Convert.ToInt32(httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var batch = new ImportBatch
            {
                DataSourceId = dataSource.DataSourceId,
                StartedAt = DateTime.UtcNow,
                Status = "Uploaded",
                ImportedBy = adminId,
                Notes = request.Notes
            };

            db.ImportBatches.Add(batch);

            await db.SaveChangesAsync(cancellationToken);

            // 3. Calculate file hash
            string fileHash;
            await using (var hashStream = request.File.OpenReadStream())
            {
                using var sha256 = SHA256.Create();

                var hash = await sha256.ComputeHashAsync(hashStream, cancellationToken);

                fileHash = Convert.ToHexString(hash).ToLowerInvariant();
            }
            
            // 4. Create storage path
            var safeFileName = Path.GetFileName(request.File.FileName);

            var storagePath = $"imports/{dataSource.DataSourceId}/" + $"{batch.ImportBatchId}/" + $"{safeFileName}";

            // 5. Upload PDF to Azure Blob
            await using var fileStream = request.File.OpenReadStream();
            await storage.UploadAsync(fileStream, storagePath, request.File.ContentType, cancellationToken);

            ImportDocument? document = null;
            try
            {
                // 6. Download and extract text from the PDF
                string extractedText;
                await using (var pdfStream = await storage.DownloadAsync(storagePath, cancellationToken))
                {
                    extractedText = await pdfTextExtractor.ExtractTextAsync(pdfStream, cancellationToken);
                }

                // 7. Create ImportDocument
                document = new ImportDocument
                {
                    ImportBatchId = batch.ImportBatchId,
                    DocumentType = "Brochure",
                    DocumentName = safeFileName,
                    SourceUrl = request.SourceUrl,
                    BlobContainer = configuration["AzureStorage:BrochureContainer"],
                    BlobPath = storagePath,
                    LocalFilePath = storagePath,
                    ContentHash = fileHash,
                    FileSizeBytes = request.File.Length,
                    MimeType = request.File.ContentType,
                    PageCount = null,
                    ExtractedText = extractedText,
                    ExtractionStatus = "Extracted",
                    CreatedAt = DateTime.UtcNow
                };
                db.ImportDocuments.Add(document);

                // 7. Update batch
                batch.Status = "TextExtracted";
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                batch.Status = "ExtractionFailed";
                batch.Notes =
                    $"{batch.Notes}\nPDF extraction failed: {ex.Message}";

                await db.SaveChangesAsync(cancellationToken);

                throw;
            }
 
            // 8. Return result
            return new BrochureUploadResponse
            {
                DataSourceId = dataSource.DataSourceId,
                ImportBatchId = batch.ImportBatchId,
                ImportDocumentId = document.ImportDocumentId,
                FileName = safeFileName,
                Status = batch.Status
            };
        }
    }
}
