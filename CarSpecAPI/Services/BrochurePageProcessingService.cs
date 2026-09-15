using CarspecAPI.Services;
using CarSpecAPI.Data.Models.DataExtractionModel;
using CarSpecAPI.Entities;
using CarSpecAPI.Services;
using CarSpecAPI.Services.OpenAIServices;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CarspecAPI.Services
{
    public class BrochurePageProcessingService : IBrochurePageProcessingService
    {
        private readonly CarsDbContext carsDbContext;
        private readonly IFileStorageService storageService;
        private readonly IPdfPageExtractor pdfPageExtractor;
        private readonly ICarBrochureAiService aiService;
        private readonly IConfiguration configuration;

        private readonly JsonSerializerOptions jsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        public BrochurePageProcessingService(
            CarsDbContext db,
            IFileStorageService storage,
            IPdfPageExtractor pdfPageExtractor,
            ICarBrochureAiService aiService,
            IConfiguration configuration)
        {
            carsDbContext = db;
            storageService = storage;
            this.pdfPageExtractor = pdfPageExtractor;
            this.aiService = aiService;
            this.configuration = configuration;
        }

        public async Task ProcessSelectedPagesAsync(
            int importDocumentId,
            string fileName,
            List<int> selectedPages,
            CancellationToken cancellationToken = default)
        {
            if (selectedPages == null || selectedPages.Count == 0)
            {
                throw new ArgumentException(
                    "At least one page must be selected.",
                    nameof(selectedPages));
            }

            // =========================================================
            // 1. Get ImportDocument
            // =========================================================

            var document =
                await carsDbContext.ImportDocuments
                    .FirstOrDefaultAsync(
                        x => x.ImportDocumentId == importDocumentId,
                        cancellationToken);

            if (document == null)
            {
                throw new InvalidOperationException(
                    $"ImportDocument {importDocumentId} was not found.");
            }

            // =========================================================
            // 2. Get ImportBatch
            // =========================================================

            var batch =
                await carsDbContext.ImportBatches
                    .FirstOrDefaultAsync(
                        x => x.ImportBatchId == document.ImportBatchId,
                        cancellationToken);

            if (batch == null)
            {
                throw new InvalidOperationException(
                    $"ImportBatch {document.ImportBatchId} was not found.");
            }

            // =========================================================
            // 3. Update status
            // =========================================================

            batch.Status = "PagesSelected";

            batch.Notes =
                $"{batch.Notes}\nSelected pages: " +
                $"{string.Join(",", selectedPages.OrderBy(x => x))}";

            await carsDbContext.SaveChangesAsync(cancellationToken);

            try
            {
                // =====================================================
                // 4. Get original PDF path
                // =====================================================

                var originalPdfPath =
                    document.BlobPath;

                // =====================================================
                // 5. Download ORIGINAL PDF from Azure
                // =====================================================

                await using var originalPdf =
                    await storageService.DownloadAsync(
                        originalPdfPath,
                        cancellationToken);

                // =====================================================
                // 6. Create temporary selected-pages PDF
                // =====================================================

                batch.Status = "PagesProcessing";

                await carsDbContext.SaveChangesAsync(cancellationToken);

                var selectedPdfBytes =
                    await pdfPageExtractor
                        .CreateSelectedPagesPdfAsync(
                            originalPdf,
                            selectedPages,
                            cancellationToken);

                // =====================================================
                // 7. Send selected PDF directly to OpenAI
                // =====================================================

                batch.Status = "AIProcessing";

                await carsDbContext.SaveChangesAsync(cancellationToken);

                var aiJson =
                    await aiService.ProcessBrochurePdfAsync(
                        selectedPdfBytes,
                        fileName,
                        selectedPages,
                        cancellationToken);

                // =====================================================
                // 8. Save AI result
                // =====================================================

                batch.AiresultJson = aiJson;

                batch.Aimodel =
                    configuration["OpenAI:ImportModel1"] + "/" +
                    configuration["OpenAI:ImportAuditModel"];

                batch.AiprocessedAt =
                    DateTime.UtcNow;

                // =====================================================
                // 9. Create ImportAuditChange rows
                // =====================================================
                //
                // IMPORTANT:
                // This is intentionally done here and NOT inside
                // GetReviewAsync().
                //
                // GET review must remain completely read-only.
                // =====================================================

                await CreateAuditChangeRowsAsync(
                    batch.ImportBatchId,
                    aiJson,
                    cancellationToken);

                // =====================================================
                // 10. AI processing completed
                // =====================================================

                batch.Status = "AICompleted";

                await carsDbContext.SaveChangesAsync(
                    cancellationToken);
            }
            catch (Exception ex)
            {
                batch.Status = "AIProcessingFailed";

                batch.Notes =
                    $"{batch.Notes}\nAI processing failed: {ex.Message}";

                await carsDbContext.SaveChangesAsync(
                    cancellationToken);

                throw;
            }
        }

        // =============================================================
        // Create ImportAuditChange database rows from AI result
        // =============================================================

        private async Task CreateAuditChangeRowsAsync(
            int importBatchId,
            string aiJson,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(aiJson))
            {
                throw new InvalidOperationException(
                    "AI returned an empty result.");
            }

            BrochureExtractionAuditResult wrapper;

            try
            {
                wrapper =
                    JsonSerializer.Deserialize<BrochureExtractionAuditResult>(
                        aiJson,
                        jsonOptions)
                    ?? throw new InvalidOperationException(
                        "AI result deserialized to null.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    "AI result JSON is invalid.",
                    ex);
            }

            if (string.IsNullOrWhiteSpace(wrapper.AuditChangesJson))
            {
                throw new InvalidOperationException(
                    "AI result does not contain AuditChangesJson.");
            }

            var auditChanges =
                DeserializeAuditChanges(
                    wrapper.AuditChangesJson);

            if (auditChanges == null)
            {
                throw new InvalidOperationException(
                    "AuditChangesJson could not be deserialized.");
            }

            // =========================================================
            // Get existing rows
            //
            // This also makes the method safe if processing is retried.
            // =========================================================

            var existingDecisions =
                await carsDbContext.ImportAuditChanges
                    .Where(x => x.ImportBatchId == importBatchId)
                    .ToListAsync(cancellationToken);

            var existingById =
                existingDecisions
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.AuditChangeId))
                    .GroupBy(
                        x => x.AuditChangeId!,
                        StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        x => x.Key,
                        x =>
                        {
                            if (x.Count() > 1)
                            {
                                throw new InvalidOperationException(
                                    $"Duplicate ImportAuditChange rows found for " +
                                    $"AuditChangeId '{x.Key}' " +
                                    $"in ImportBatch {importBatchId}.");
                            }

                            return x.Single();
                        },
                        StringComparer.OrdinalIgnoreCase);

            // =========================================================
            // Create missing rows
            // =========================================================

            foreach (var change in auditChanges)
            {
                if (string.IsNullOrWhiteSpace(change.ChangeId))
                {
                    throw new InvalidOperationException(
                        "An AI audit change has an empty ChangeId.");
                }

                if (change.Target == null)
                {
                    throw new InvalidOperationException(
                        $"Audit change '{change.ChangeId}' has null target.");
                }

                if (existingById.ContainsKey(change.ChangeId))
                    continue;

                var identity = change.Target?.Identity?.Trim();

                if (!string.IsNullOrEmpty(identity) && identity.Length > 200)
                {
                    throw new InvalidOperationException(
                        $"Audit change {change.ChangeId} has an invalidly long target identity " +
                        $"({identity.Length} characters): {identity}");
                }

                var targetCollection =
                    NormalizeCollectionName(
                        change.Target.Collection);

                var auditEntity =
                    new ImportAuditChange
                    {
                        ImportBatchId = importBatchId,

                        // AI change ID such as CHG-001
                        AuditChangeId = change.ChangeId,

                        Action = change.Action,

                        EntityType = targetCollection,

                        EntityKey = change.Target.Identity,

                        FieldName = null,

                        OldData = null,

                        NewData = change.Data.HasValue
                                        ? change.Data.Value.GetRawText()
                                        : null,

                        Reason = change.Reason,

                        Status = "Pending",

                        ReviewedBy = null,

                        ReviewedAt = null,

                        Notes = null
                    };

                carsDbContext.ImportAuditChanges.Add(
                    auditEntity);

                existingDecisions.Add(
                    auditEntity);

                existingById.Add(
                    change.ChangeId,
                    auditEntity);
            }

            // =========================================================
            // Save only if new audit rows were created
            // =========================================================

            if (carsDbContext.ChangeTracker.HasChanges())
            {
                await carsDbContext.SaveChangesAsync(
                    cancellationToken);
            }
        }

        // =============================================================
        // Deserialize AuditChangesJson
        // =============================================================

        private List<AuditChangeDto> DeserializeAuditChanges(
            string? auditChangesJson)
        {
            if (string.IsNullOrWhiteSpace(auditChangesJson))
            {
                return new List<AuditChangeDto>();
            }

            try
            {
                /*
                 * Current format:
                 *
                 * {
                 *     "changes": [...]
                 * }
                 */
                using var document =
                    JsonDocument.Parse(auditChangesJson);

                if (document.RootElement.ValueKind ==
                    JsonValueKind.Object)
                {
                    if (document.RootElement.TryGetProperty(
                        "changes",
                        out var changesElement))
                    {
                        return
                            JsonSerializer.Deserialize<List<AuditChangeDto>>(
                                changesElement.GetRawText(),
                                jsonOptions)
                            ?? new List<AuditChangeDto>();
                    }
                }

                /*
                 * Legacy fallback:
                 *
                 * [
                 *     {...},
                 *     {...}
                 * ]
                 */
                if (document.RootElement.ValueKind ==
                    JsonValueKind.Array)
                {
                    return
                        JsonSerializer.Deserialize<List<AuditChangeDto>>(
                            document.RootElement.GetRawText(),
                            jsonOptions)
                        ?? new List<AuditChangeDto>();
                }

                throw new InvalidOperationException(
                    "AuditChangesJson must contain a 'changes' array.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    "AuditChangesJson is invalid.",
                    ex);
            }
        }

        // =============================================================
        // Normalize collection name
        // =============================================================

        private string NormalizeCollectionName(
            string? collection)
        {
            if (string.IsNullOrWhiteSpace(collection))
            {
                throw new InvalidOperationException(
                    "Audit change target collection is empty.");
            }

            return collection.Trim();
        }
    }
}