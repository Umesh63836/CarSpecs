using CarSpecAPI.Data.Models.DataExtractionModel;
using CarSpecAPI.Entities;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Threading.Channels;

namespace CarspecAPI.Services;

public class AdminReviewService
{
    private readonly CarsDbContext carsDbContext;

    private readonly JsonSerializerOptions _jsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true
        };

    public AdminReviewService(CarsDbContext db)
    {
        carsDbContext = db;
    }

    // ============================================================
    // LOAD REVIEW
    // ============================================================

    public async Task<BrochureImportReviewDto> GetReviewAsync(
        int importBatchId,
        CancellationToken cancellationToken = default)
    {
        if (importBatchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(importBatchId));

        // =========================================================
        // 1. Get ImportBatch
        // =========================================================

        var batch =
            await carsDbContext.ImportBatches
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.ImportBatchId == importBatchId,
                    cancellationToken);

        if (batch == null)
        {
            throw new InvalidOperationException(
                $"ImportBatch {importBatchId} was not found.");
        }

        // =========================================================
        // 2. Validate AI result
        // =========================================================

        if (string.IsNullOrWhiteSpace(batch.AiresultJson))
        {
            throw new InvalidOperationException(
                $"ImportBatch {importBatchId} does not contain AI result.");
        }

        // =========================================================
        // 3. Deserialize AI wrapper
        // =========================================================

        BrochureExtractionAuditResult wrapper;

        try
        {
            wrapper =
                JsonSerializer.Deserialize<BrochureExtractionAuditResult>(
                    batch.AiresultJson,
                    _jsonOptions)
                ?? throw new InvalidOperationException(
                    "AI result deserialized to null.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"AI result JSON is invalid for ImportBatch " +
                $"{importBatchId}.",
                ex);
        }

        // =========================================================
        // 4. Get Model1 JSON
        // =========================================================

        if (string.IsNullOrWhiteSpace(wrapper.Model1Json))
        {
            throw new InvalidOperationException(
                "AI result does not contain Model1Json.");
        }

        BrochureExtractionDto model1;

        try
        {
            model1 =
                JsonSerializer.Deserialize<BrochureExtractionDto>(
                    wrapper.Model1Json,
                    _jsonOptions)
                ?? throw new InvalidOperationException(
                    "Model1 JSON deserialized to null.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                "Model1Json is invalid.",
                ex);
        }

        // =========================================================
        // 5. Deserialize and validate AI audit changes
        // =========================================================

        var auditChanges =
            DeserializeAuditChanges(
                wrapper.AuditChangesJson);

        ValidateAuditChanges(auditChanges);

        // =========================================================
        // 6. READ existing admin decisions
        // =========================================================

        var existingDecisions =
            await carsDbContext.ImportAuditChanges
                .AsNoTracking()
                .Where(x =>
                    x.ImportBatchId == importBatchId)
                .ToListAsync(cancellationToken);

        // =========================================================
        // 7. Build DB decision lookup
        // =========================================================

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
        // 8. Build review DTO
        // =========================================================

        var reviewChanges =
            new List<AuditChangeReviewDto>();

        foreach (var change in auditChanges)
        {
            if (change.Target == null)
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' has null target.");
            }

            existingById.TryGetValue(
                change.ChangeId,
                out var existing);

            reviewChanges.Add(
                new AuditChangeReviewDto
                {
                    ChangeId =
                        change.ChangeId,

                    IssueType =
                        change.IssueType,

                    Category =
                        change.Category,

                    Severity =
                        change.Severity,

                    Action =
                        change.Action,

                    Collection =
                        NormalizeCollectionName(
                            change.Target.Collection),

                    IndexId =
                        change.Target.IndexId,

                    Identity =
                        change.Target.Identity ??
                        string.Empty,

                    Data =
                        change.Data,

                    PageNumber =
                        change.PageNumber,

                    Evidence =
                        change.Evidence,

                    Reason =
                        change.Reason,

                    Decision =
                        string.IsNullOrWhiteSpace(
                            existing?.Status)
                            ? "Pending"
                            : existing!.Status!,

                    ModifiedData =
                            existing?.Status == "Modified"
                                ? existing.NewData
                                : null,

                    Notes =
                        existing?.Notes
                });
        }

        // =========================================================
        // 9. Return review
        // =========================================================

        return new BrochureImportReviewDto
        {
            ImportBatchId =
                importBatchId,

            Status =
                batch.Status ??
                string.Empty,

            Model1 =
                model1,

            AuditChanges =
                reviewChanges,

            Warnings =
                model1.Warnings ??
                new List<string>()
        };
    }


    // ============================================================
    // SAVE ADMIN DECISIONS
    // ============================================================

    public async Task SaveReviewAsync(
        int importBatchId,
        List<AdminAuditDecisionDto> decisions,
        int? reviewedBy,
        CancellationToken cancellationToken = default)
    {
        if (importBatchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(importBatchId));

        if (decisions == null)
            throw new ArgumentNullException(nameof(decisions));

        var batch =
            await carsDbContext.ImportBatches
                .FirstOrDefaultAsync(
                    x => x.ImportBatchId == importBatchId,
                    cancellationToken);

        if (batch == null)
        {
            throw new InvalidOperationException(
                $"ImportBatch {importBatchId} was not found.");
        }

        if (!string.Equals(
                batch.Status,
                "AICompleted",
                StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(
                batch.Status,
                "AdminReview",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Batch {importBatchId} is not available for admin review. " +
                $"Current status: {batch.Status}");
        }

        if (decisions.Count == 0)
        {
            throw new InvalidOperationException(
                "No admin decisions were supplied.");
        }

        // =========================================================
        // Read original AI changes
        // =========================================================

        if (string.IsNullOrWhiteSpace(batch.AiresultJson))
        {
            throw new InvalidOperationException(
                "AI result is missing.");
        }

        BrochureExtractionAuditResult wrapper;

        try
        {
            wrapper =
                JsonSerializer.Deserialize<BrochureExtractionAuditResult>(
                    batch.AiresultJson,
                    _jsonOptions)
                ?? throw new InvalidOperationException(
                    "AI result deserialized to null.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                "AI result JSON is invalid.",
                ex);
        }

        var aiChanges =
            DeserializeAuditChanges(
                wrapper.AuditChangesJson);

        ValidateAuditChanges(aiChanges);

        var aiById =
            aiChanges.ToDictionary(
                x => x.ChangeId,
                StringComparer.OrdinalIgnoreCase);

        // =========================================================
        // Validate duplicate submitted decisions
        // =========================================================

        var decisionIds =
            decisions
                .Select(x => x.ChangeId)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

        var duplicateDecisionIds =
            decisionIds
                .GroupBy(
                    x => x,
                    StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

        if (duplicateDecisionIds.Count > 0)
        {
            throw new InvalidOperationException(
                "Duplicate audit decisions were supplied: " +
                string.Join(", ", duplicateDecisionIds));
        }

        // =========================================================
        // Get DB audit changes
        // =========================================================

        var dbChanges =
            await carsDbContext.ImportAuditChanges
                .Where(x => x.ImportBatchId == importBatchId)
                .ToListAsync(cancellationToken);

        var dbById =
            dbChanges
                .Where(x => !string.IsNullOrWhiteSpace(x.AuditChangeId))
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
                                $"Duplicate ImportAuditChange rows found " +
                                $"for AuditChangeId '{x.Key}'.");
                        }

                        return x.Single();
                    },
                    StringComparer.OrdinalIgnoreCase);

        // =========================================================
        // Apply decisions
        // =========================================================

        foreach (var decision in decisions)
        {
            if (string.IsNullOrWhiteSpace(decision.ChangeId))
            {
                throw new InvalidOperationException(
                    "An admin decision contains an empty ChangeId.");
            }

            if (!aiById.TryGetValue(
                    decision.ChangeId,
                    out var aiChange))
            {
                throw new InvalidOperationException(
                    $"Audit change '{decision.ChangeId}' " +
                    "does not exist in AI audit result.");
            }

            if (!dbById.TryGetValue(
                    decision.ChangeId,
                    out var auditChange))
            {
                throw new InvalidOperationException(
                    $"Audit change '{decision.ChangeId}' " +
                    "does not exist in ImportAuditChange.");
            }

            var decisionValue =
                NormalizeDecision(
                    decision.Decision);

            switch (decisionValue)
            {
                case "Accepted":

                    // Accepted = use exactly the original AI data.
                    auditChange.NewData =
                        aiChange.Data.HasValue
                            ? aiChange.Data.Value.GetRawText()
                            : null;

                    break;

                case "Rejected":

                    // Rejected = never apply.
                    auditChange.NewData = null;

                    break;

                case "Modified":

                    if (!HasJsonData(decision.ModifiedData))
                    {
                        throw new InvalidOperationException(
                            $"Audit change '{decision.ChangeId}' " +
                            "is marked Modified but ModifiedData is empty.");
                    }

                    ValidateModifiedDataJson(
                        decision.ModifiedData,
                        aiChange,
                        decision.ChangeId);

                    auditChange.NewData =
                        decision.ModifiedData!.Value.GetRawText();

                    break;

                default:

                    throw new InvalidOperationException(
                        $"Invalid audit decision '{decision.Decision}' " +
                        $"for AuditChangeId '{decision.ChangeId}'.");
            }

            auditChange.Status =
                decisionValue;

            auditChange.ReviewedBy =
                reviewedBy;

            auditChange.ReviewedAt =
                DateTime.UtcNow;

            if (decision.Notes != null)
            {
                auditChange.Notes =
                    decision.Notes;
            }
        }

        batch.Status = "AdminReview";

        await carsDbContext.SaveChangesAsync(
            cancellationToken);
    }


    // ============================================================
    // BUILD FINAL JSON
    // ============================================================

    public async Task<string> BuildFinalJsonAsync(
        int importBatchId,
        CancellationToken cancellationToken = default)
    {
        if (importBatchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(importBatchId));

        var batch =
            await carsDbContext.ImportBatches
                .FirstOrDefaultAsync(
                    x => x.ImportBatchId == importBatchId,
                    cancellationToken);

        if (batch == null)
        {
            throw new InvalidOperationException(
                $"ImportBatch {importBatchId} was not found.");
        }

        if (!string.Equals(
                batch.Status,
                "AICompleted",
                StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(
                batch.Status,
                "AdminReview",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Review not allowed for batch {importBatchId}." +
                $" Current status: {batch.Status}");
        }

        if (string.IsNullOrWhiteSpace(
                batch.AiresultJson))
        {
            throw new InvalidOperationException(
                "AI result is missing.");
        }

        BrochureExtractionAuditResult wrapper;

        try
        {
            wrapper =
                JsonSerializer.Deserialize<BrochureExtractionAuditResult>(
                    batch.AiresultJson,
                    _jsonOptions)
                ?? throw new InvalidOperationException(
                    "AI result deserialized to null.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                "AI result JSON is invalid.",
                ex);
        }

        if (string.IsNullOrWhiteSpace(
                wrapper.Model1Json))
        {
            throw new InvalidOperationException(
                "Model1Json is missing.");
        }

        BrochureExtractionDto model;

        try
        {
            model =
                JsonSerializer.Deserialize<BrochureExtractionDto>(
                    wrapper.Model1Json,
                    _jsonOptions)
                ?? throw new InvalidOperationException(
                    "Model1 JSON deserialized to null.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                "Model1Json is invalid.",
                ex);
        }

        var aiChanges =
            DeserializeAuditChanges(
                wrapper.AuditChangesJson);

        ValidateAuditChanges(aiChanges);

        /*
         * IMPORTANT:
         *
         * Do NOT order by AuditChangeId.
         *
         * AuditChangeId is the AI string ID.
         *
         * The target.indexId belongs to the ORIGINAL Model1 JSON and is
         * therefore taken from aiChange.Target.IndexId.
         */

        var dbChanges =
            await carsDbContext.ImportAuditChanges
                .Where(x =>
                    x.ImportBatchId == importBatchId)
                .ToListAsync(cancellationToken);

        var dbById =
            dbChanges
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.AuditChangeId))
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
                                $"Duplicate ImportAuditChange rows found " +
                                $"for AuditChangeId '{x.Key}'.");
                        }

                        return x.Single();
                    },
                    StringComparer.OrdinalIgnoreCase);

        foreach (var aiChange in aiChanges)
        {
            if (!dbById.TryGetValue(
                    aiChange.ChangeId,
                    out var dbChange))
            {
                throw new InvalidOperationException(
                    $"Audit change '{aiChange.ChangeId}' " +
                    "exists in AI result but not in ImportAuditChange.");
            }

            var status =
                NormalizeDecision(
                    dbChange.Status);

            switch (status)
            {
                case "Accepted":

                    /*
                     * Accepted always uses original AI data.
                     *
                     * IMPORTANT:
                     * Pass the original AI target.indexId.
                     */
                    ApplyChange(
                        model,
                        dbChange,
                        aiChange.Target?.IndexId,
                            aiChange.Data.HasValue
                                        ? aiChange.Data.Value.GetRawText()
                                        : null);

                    break;

                case "Modified":

                    if (string.IsNullOrWhiteSpace(
                            dbChange.NewData))
                    {
                        throw new InvalidOperationException(
                            $"Audit change '{dbChange.AuditChangeId}' " +
                            "is Modified but NewData is empty.");
                    }

                    /*
                     * Modified also uses the original AI target.indexId.
                     */
                    ApplyChange(
                        model,
                        dbChange,
                        aiChange.Target?.IndexId,
                        dbChange.NewData);

                    break;

                case "Rejected":

                    // Explicitly do nothing.
                    break;

                case "Pending":

                    throw new InvalidOperationException(
                        $"Audit change '{dbChange.AuditChangeId}' " +
                        "has not been reviewed.");

                default:

                    throw new InvalidOperationException(
                        $"Invalid audit decision '{dbChange.Status}' " +
                        $"for AuditChangeId '{dbChange.AuditChangeId}'.");
            }
        }

        var result =
            JsonSerializer.Serialize(
                model,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        //Console.WriteLine($"FINAL JSON LENGTH BEFORE DB SAVE: {result.Length}");

        //try
        //{
        //    JsonSerializer.Deserialize<JsonElement>(result);
        //    Console.WriteLine("FINAL JSON IS VALID");
        //}
        //catch (JsonException ex)
        //{
        //    Console.WriteLine($"FINAL JSON IS INVALID: {ex.Message}");
        //}

        await UpdateBatchAsync(
            importBatchId,
            result,
            cancellationToken);

        return result;
    }



    private async Task UpdateBatchAsync(
        int batchId,
        string result,
        CancellationToken cancellationToken)
    {
        var batch =
            await carsDbContext.ImportBatches
                .FirstOrDefaultAsync(
                    i => i.ImportBatchId == batchId);

        if (batch != null)
        {
            batch.FinalJson = result;
            batch.Status = "Reviewed&Merged";

            await carsDbContext.SaveChangesAsync(
                cancellationToken);
        }
    }


    // ============================================================
    // DESERIALIZE AUDIT CHANGES
    // ============================================================

    private static List<AuditChangeDto> DeserializeAuditChanges(
        string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<AuditChangeDto>();

        var options =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        try
        {
            /*
             * Current auditor format:
             *
             * {
             *   "changes": [...]
             * }
             */

            var wrapper =
                JsonSerializer.Deserialize<AuditChangesContainerDto>(
                    json,
                    options);

            if (wrapper?.Changes != null)
                return wrapper.Changes;
        }
        catch (JsonException)
        {
            // Try legacy raw-array format below.
        }

        try
        {
            /*
             * Backward compatibility for old raw-array format.
             */

            return
                JsonSerializer.Deserialize<List<AuditChangeDto>>(
                    json,
                    options)
                ?? new List<AuditChangeDto>();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                "AuditChangesJson is not a valid audit-change object or array.",
                ex);
        }
    }


    // ============================================================
    // VALIDATE AUDIT CHANGES
    // ============================================================

    private static void ValidateAuditChanges(
        List<AuditChangeDto> changes)
    {
        var ids =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var change in changes)
        {
            if (string.IsNullOrWhiteSpace(
                    change.ChangeId))
            {
                throw new InvalidOperationException(
                    "An AI audit change has an empty changeId.");
            }

            if (!ids.Add(change.ChangeId))
            {
                throw new InvalidOperationException(
                    $"Duplicate AI audit changeId '{change.ChangeId}'.");
            }

            if (change.Target == null)
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' has no target.");
            }

            if (string.IsNullOrWhiteSpace(
                    change.Target.Collection))
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' has no target collection.");
            }

            var collection =
                NormalizeCollectionName(
                    change.Target.Collection);

            if (collection != "vehicle" &&
                collection != "document" &&
                string.IsNullOrWhiteSpace(
                    change.Target.Identity))
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' " +
                    $"targeting '{collection}' has no identity.");
            }

            if (!IsSupportedCollection(
                    collection))
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' targets unsupported " +
                    $"collection '{change.Target.Collection}'.");
            }

            // =====================================================
            // IndexId validation
            // =====================================================

            if (string.Equals(
                    change.Action,
                    "Add",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (change.Target.IndexId.HasValue)
                {
                    throw new InvalidOperationException(
                        $"Audit change '{change.ChangeId}' is Add and " +
                        "must have target.indexId = null.");
                }
            }

            if (string.Equals(
                    change.Action,
                    "Replace",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    change.Action,
                    "Remove",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (collection != "vehicle" &&
                    collection != "document" &&
                    collection != "warnings" &&
                    !change.Target.IndexId.HasValue)
                {
                    throw new InvalidOperationException(
                        $"Audit change '{change.ChangeId}' is " +
                        $"{change.Action} and must contain target.indexId.");
                }
            }

            if (string.Equals(
                    change.Action,
                    "Add",
                    StringComparison.OrdinalIgnoreCase))
            {
                ValidateAuditChangeData(
                    change,
                    collection,
                    requireData: true);
            }

            if (string.Equals(
                    change.Action,
                    "Replace",
                    StringComparison.OrdinalIgnoreCase))
            {
                ValidateAuditChangeData(
                    change,
                    collection,
                    requireData: true);
            }

            if (string.Equals(
                    change.Action,
                    "Remove",
                    StringComparison.OrdinalIgnoreCase) &&
                HasJsonData(change.Data))
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' is Remove " +
                    "and must have data = null.");
            }

            if (!string.Equals(
                    change.Action,
                    "Add",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(
                    change.Action,
                    "Replace",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(
                    change.Action,
                    "Remove",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' has unsupported " +
                    $"action '{change.Action}'.");
            }
        }
    }


    private static bool HasJsonData(JsonElement? data)
    {
        return data.HasValue &&
               data.Value.ValueKind != JsonValueKind.Null &&
               data.Value.ValueKind != JsonValueKind.Undefined;
    }


    private static JsonElement? ParseStoredJsonData(
        string? json,
        string changeId)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            using var document =
                JsonDocument.Parse(json);

            return document.RootElement.Clone();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Stored NewData for audit change '{changeId}' is not valid JSON.",
                ex);
        }
    }


    private static void ValidateAuditChangeData(
        AuditChangeDto change,
        string collection,
        bool requireData)
    {
        if (!requireData)
            return;

        if (!HasJsonData(change.Data))
        {
            throw new InvalidOperationException(
                $"Audit change '{change.ChangeId}' " +
                $"is {change.Action} but data is empty.");
        }

        var data = change.Data!.Value;

        if (collection == "warnings")
        {
            if (data.ValueKind != JsonValueKind.String)
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' targeting warnings " +
                    "must contain data as a JSON string.");
            }

            return;
        }

        if (data.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException(
                $"Audit change '{change.ChangeId}' targeting '{collection}' " +
                "must contain data as a JSON object.");
        }

        var action = change.Action;

        if (string.Equals(action, "Add", StringComparison.OrdinalIgnoreCase))
        {
            if (data.TryGetProperty("indexId", out _))
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' is Add and its data " +
                    "must NOT contain indexId.");
            }

            return;
        }

        if (string.Equals(action, "Replace", StringComparison.OrdinalIgnoreCase))
        {
            if (!change.Target.IndexId.HasValue)
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' is Replace and " +
                    "target.indexId is missing.");
            }

            if (!data.TryGetProperty("indexId", out var indexIdElement) ||
                indexIdElement.ValueKind != JsonValueKind.Number ||
                !indexIdElement.TryGetInt32(out var dataIndexId) ||
                dataIndexId <= 0)
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' is Replace and its " +
                    "data must contain a positive integer indexId.");
            }

            if (dataIndexId != change.Target.IndexId.Value)
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.ChangeId}' replacement data indexId " +
                    $"{dataIndexId} does not match target.indexId " +
                    $"{change.Target.IndexId.Value}.");
            }
        }
    }


    // ============================================================
    // APPLY CHANGE
    // ============================================================

    private void ApplyChange(
        BrochureExtractionDto model,
        ImportAuditChange change,
        int? targetIndexId,
        string? data)
    {
        if (string.Equals(
                change.Action,
                "Add",
                StringComparison.OrdinalIgnoreCase))
        {
            ApplyAdd(
                model,
                change,
                data);

            return;
        }

        if (string.Equals(
                change.Action,
                "Replace",
                StringComparison.OrdinalIgnoreCase))
        {
            ApplyReplace(
                model,
                change,
                targetIndexId,
                data);

            return;
        }

        if (string.Equals(
                change.Action,
                "Remove",
                StringComparison.OrdinalIgnoreCase))
        {
            ApplyRemove(
                model,
                change,
                targetIndexId);

            return;
        }

        throw new InvalidOperationException(
            $"Unsupported audit action '{change.Action}'.");
    }


    // ============================================================
    // APPLY ADD
    // ============================================================

    private void ApplyAdd(
        BrochureExtractionDto model,
        ImportAuditChange change,
        string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' " +
                "is Add but NewData is empty.");
        }

        var collection =
            NormalizeCollectionName(
                change.EntityType);

        /*
         * Vehicle/document are objects, not collections.
         * Add is therefore invalid.
         */
        if (collection == "vehicle" ||
            collection == "document")
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' cannot Add " +
                $"to object collection '{collection}'.");
        }

        var list =
            GetCollection(
                model,
                collection,
                out var itemType);

        var identity =
            change.EntityKey;

        if (string.IsNullOrWhiteSpace(identity))
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' has no EntityKey.");
        }

        object? newItem;

        try
        {
            if (itemType == typeof(string))
            {
                newItem =
                    DeserializeStringData(
                        data);
            }
            else
            {
                /*
                 * ADD objects do not have an indexId yet.
                 *
                 * The AI should normally omit indexId completely.
                 * However, if the AI sends:
                 *
                 *     "indexId": null
                 *
                 * remove it before deserialization so that a nullable
                 * JSON value cannot be assigned to the non-nullable
                 * IndexId property.
                 *
                 * The backend assigns the real IndexId below.
                 */
                using var document =
                    JsonDocument.Parse(data);

                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException(
                        $"Audit change '{change.AuditChangeId}' " +
                        $"Add data for collection '{collection}' " +
                        "must be a JSON object.");
                }

                var jsonObject =
                    new Dictionary<string, JsonElement>(
                        StringComparer.OrdinalIgnoreCase);

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    /*
                     * Never allow the AI to supply an IndexId for Add.
                     *
                     * This removes both:
                     *     "indexId": null
                     *
                     * and, defensively:
                     *     "indexId": 123
                     *
                     * The backend owns IndexId assignment.
                     */
                    if (string.Equals(
                            property.Name,
                            "indexId",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    jsonObject[property.Name] =
                        property.Value.Clone();
                }

                var cleanedData =
                    JsonSerializer.Serialize(
                        jsonObject,
                        _jsonOptions);

                newItem =
                    JsonSerializer.Deserialize(
                        cleanedData,
                        itemType,
                        _jsonOptions);
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' contains invalid " +
                $"data for collection '{collection}'." +
                $"JSON error: {ex.Message}",
                ex);
        }

        if (newItem == null)
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' produced null data.");
        }

        /*
         * The canonical identity must match the identity supplied by AI.
         *
         * IMPORTANT:
         * We intentionally DO NOT check whether another item already
         * has the same identity.
         *
         * Duplicate identities are valid for collections such as:
         *
         * features
         * specifications
         * fuelEfficiencies
         *
         * where multiple observations can legitimately share the same
         * canonical identity but differ in value/applicability.
         */
        if (!IdentityMatches(
                newItem,
                collection,
                identity))
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' identity mismatch. " +
                $"Target identity '{identity}' does not match supplied data.");
        }

        /*
         * New occurrences receive a new IndexId.
         *
         * IMPORTANT:
         * Never use list.Count because previously removed IndexIds
         * must not be reused.
         *
         * Example:
         *
         * Existing IndexIds:
         *     1, 2, 3, 5
         *
         * If 5 was previously removed, the next Add must still receive:
         *     6
         *
         * not:
         *     5
         */
        if (itemType != typeof(string))
        {
            var property =
                GetIndexIdProperty(newItem);

            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Collection '{collection}' item type " +
                    $"'{itemType.Name}' does not contain indexId.");
            }

            var newItemIndexId =
                GetNextIndexId(list);

            property.SetValue(
                newItem,
                newItemIndexId);
        }

        list.Add(newItem);
    }


    // ============================================================
    // APPLY REPLACE
    // ============================================================

    private void ApplyReplace(
        BrochureExtractionDto model,
        ImportAuditChange change,
        int? targetIndexId,
        string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' " +
                "is Replace but NewData is empty.");
        }

        var collection =
            NormalizeCollectionName(
                change.EntityType);

        /*
         * Vehicle/document are objects rather than arrays.
         */
        if (collection == "vehicle" ||
            collection == "document")
        {
            ReplaceObjectProperty(
                model,
                collection,
                data,
                change);

            return;
        }

        var list =
            GetCollection(
                model,
                collection,
                out var itemType);

        if (string.IsNullOrWhiteSpace(
                change.EntityKey))
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' " +
                "has no EntityKey.");
        }

        if (!targetIndexId.HasValue)
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' " +
                $"cannot Replace '{change.EntityKey}' in '{collection}' " +
                "because target.indexId is missing.");
        }

        var targetIndexIdValue =
            targetIndexId.Value;

        var index =
            FindIndexByIndexId(
                list,
                collection,
                targetIndexIdValue,
                change.EntityKey!,
                change.AuditChangeId!);

        object? replacement;

        try
        {
            if (itemType == typeof(string))
            {
                replacement =
                    DeserializeStringData(
                        data);
            }
            else
            {
                replacement =
                    JsonSerializer.Deserialize(
                        data,
                        itemType,
                        _jsonOptions);
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' contains invalid " +
                $"replacement data for '{collection}'.",
                ex);
        }

        if (replacement == null)
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' produced null replacement.");
        }

        /*
         * The replacement must retain the same canonical identity.
         */
        if (!IdentityMatches(
                replacement,
                collection,
                change.EntityKey))
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' replacement identity " +
                $"does not match target identity '{change.EntityKey}'.");
        }

        if (itemType != typeof(string))
        {
            var replacementIndexId =
                GetIndexId(
                    replacement);

            if (!replacementIndexId.HasValue ||
                replacementIndexId.Value != targetIndexIdValue)
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.AuditChangeId}' replacement " +
                    $"must retain indexId {targetIndexIdValue}.");
            }
        }

        list[index] = replacement;
    }


    // ============================================================
    // APPLY REMOVE
    // ============================================================

    private void ApplyRemove(
        BrochureExtractionDto model,
        ImportAuditChange change,
        int? targetIndexId)
    {
        var collection =
            NormalizeCollectionName(
                change.EntityType);

        if (collection == "vehicle" ||
            collection == "document")
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' cannot Remove " +
                $"object '{collection}'.");
        }

        var list =
            GetCollection(
                model,
                collection,
                out _);

        if (collection == "warnings")
        {
            var warningIndex =
                FindCollectionIndex(
                    list,
                    collection,
                    change.EntityKey ?? string.Empty);

            if (warningIndex < 0)
            {
                throw new InvalidOperationException(
                    $"Audit change '{change.AuditChangeId}' cannot Remove " +
                    $"warning '{change.EntityKey}' because it was not found.");
            }

            list.RemoveAt(warningIndex);
            return;
        }

        if (string.IsNullOrWhiteSpace(
                change.EntityKey))
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' " +
                "has no EntityKey.");
        }

        if (!targetIndexId.HasValue)
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' " +
                $"cannot Remove '{change.EntityKey}' from '{collection}' " +
                "because target.indexId is missing.");
        }

        var index =
            FindIndexByIndexId(
                list,
                collection,
                targetIndexId.Value,
                change.EntityKey!,
                change.AuditChangeId!);

        list.RemoveAt(index);
    }


    // ============================================================
    // TARGET INDEX ID MATCHING
    // ============================================================

    private static int FindIndexByIndexId(
        IList list,
        string collection,
        int indexId,
        string identity,
        string changeId)
    {
        if (indexId <= 0)
        {
            throw new InvalidOperationException(
                $"Audit change '{changeId}' has invalid indexId " +
                $"{indexId}.");
        }

        var matches = new List<int>();

        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];

            if (item == null)
                continue;

            var itemIndexId =
                GetIndexId(item);

            if (itemIndexId == indexId)
            {
                matches.Add(i);
            }
        }

        if (matches.Count == 0)
        {
            throw new InvalidOperationException(
                $"Audit change '{changeId}' targets indexId {indexId} " +
                $"in collection '{collection}', but no matching item exists.");
        }

        if (matches.Count > 1)
        {
            throw new InvalidOperationException(
                $"Audit change '{changeId}' targets indexId {indexId} " +
                $"in collection '{collection}', but multiple items have " +
                $"that indexId.");
        }

        var index = matches[0];
        var itemAtTarget = list[index];

        if (itemAtTarget == null ||
            !IdentityMatches(
                itemAtTarget,
                collection,
                identity))
        {
            throw new InvalidOperationException(
                $"Audit change '{changeId}' targets indexId {indexId} " +
                $"in collection '{collection}', but that item does not " +
                $"match identity '{identity}'.");
        }

        return index;
    }


    private static int? GetIndexId(object item)
    {
        var property =
            GetIndexIdProperty(item);

        if (property == null)
            return null;

        var value =
            property.GetValue(item);

        if (value == null)
            return null;

        try
        {
            return Convert.ToInt32(
                value,
                CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }


    private static PropertyInfo? GetIndexIdProperty(
        object item)
    {
        return item.GetType()
            .GetProperty(
                "IndexId",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.IgnoreCase);
    }


    private static int GetNextIndexId(
        IList list)
    {
        var maxIndexId = 0;

        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];

            if (item == null)
                continue;

            var indexId =
                GetIndexId(item);

            if (indexId.HasValue &&
                indexId.Value > maxIndexId)
            {
                maxIndexId = indexId.Value;
            }
        }

        return maxIndexId + 1;
    }


    // ============================================================
    // GENERIC INDEX FINDER
    // ============================================================

    /*
     * Identity-only lookup is retained for compatibility with the
     * helper methods below.
     *
     * IMPORTANT:
     * Identity is NOT required to be unique.
     *
     * This method returns the first matching item and is retained only
     * for compatibility with older helper paths.
     */
    private static int FindCollectionIndex(
        IList list,
        string collection,
        string identity)
    {
        if (string.IsNullOrWhiteSpace(identity))
            return -1;

        for (var i = 0; i < list.Count; i++)
        {
            var item =
                list[i];

            if (item == null)
                continue;

            if (IdentityMatches(
                    item,
                    collection,
                    identity))
            {
                return i;
            }
        }

        return -1;
    }


    // ============================================================
    // IDENTITY MATCHING
    // ============================================================

    private static bool IdentityMatches(
        object item,
        string collection,
        string identity)
    {
        if (item is string stringItem)
        {
            return string.Equals(
                stringItem.Trim(),
                identity.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        var candidates =
            GetIdentityCandidates(
                item,
                collection);

        return candidates.Any(
            x =>
                string.Equals(
                    x,
                    identity.Trim(),
                    StringComparison.OrdinalIgnoreCase));
    }


    private static IEnumerable<string> GetIdentityCandidates(
        object item,
        string collection)
    {
        collection =
            NormalizeCollectionName(
                collection);

        switch (collection)
        {
            case "powertrains":

                AddCandidate(
                    GetStringProperty(
                        item,
                        "PowertrainRef"),
                    out var powertrainRef);

                if (powertrainRef != null)
                    yield return powertrainRef;

                break;


            case "engines":

                AddCandidate(
                    GetStringProperty(
                        item,
                        "EngineRef"),
                    out var engineRef);

                if (engineRef != null)
                    yield return engineRef;

                break;


            case "transmissions":

                AddCandidate(
                    GetStringProperty(
                        item,
                        "TransmissionRef"),
                    out var transmissionRef);

                if (transmissionRef != null)
                    yield return transmissionRef;

                break;


            case "drivetrains":

                AddCandidate(
                    GetStringProperty(
                        item,
                        "DrivetrainRef"),
                    out var drivetrainRef);

                if (drivetrainRef != null)
                    yield return drivetrainRef;

                break;


            case "variants":

                AddCandidate(
                    GetStringProperty(
                        item,
                        "VariantName"),
                    out var variantName);

                if (variantName != null)
                    yield return variantName;

                break;


            case "features":

                var featureCode =
                    GetStringProperty(
                        item,
                        "FeatureCode");

                if (!string.IsNullOrWhiteSpace(featureCode))
                    yield return featureCode.Trim();

                break;


            case "specifications":

                var specificationCode =
                    GetStringProperty(
                        item,
                        "SpecificationCode");

                if (!string.IsNullOrWhiteSpace(
                        specificationCode))
                {
                    yield return specificationCode.Trim();
                }

                break;


            case "fuelEfficiencies":

                var fuelEfficiency =
                    GetStringProperty(
                        item,
                        "FuelEfficiency");

                if (!string.IsNullOrWhiteSpace(
                        fuelEfficiency))
                {
                    yield return fuelEfficiency.Trim();
                }

                break;


            case "warnings":

                /*
                 * Warnings are not part of the canonical identity list.
                 *
                 * They are retained here only for backward compatibility
                 * with the existing collection structure.
                 */
                if (item is string warning)
                    yield return warning.Trim();

                break;
        }
    }


    // ============================================================
    // COLLECTION ACCESS
    // ============================================================

    private static IList GetCollection(
        BrochureExtractionDto model,
        string collection,
        out Type itemType)
    {
        collection =
            NormalizeCollectionName(
                collection);

        var property =
            typeof(BrochureExtractionDto)
                .GetProperty(
                    collection,
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.IgnoreCase);

        if (property == null)
        {
            throw new InvalidOperationException(
                $"BrochureExtractionDto does not contain collection " +
                $"'{collection}'.");
        }

        var value =
            property.GetValue(model);

        if (value == null)
        {
            throw new InvalidOperationException(
                $"Collection '{collection}' is null.");
        }

        if (value is not IList list)
        {
            throw new InvalidOperationException(
                $"Collection '{collection}' must implement IList.");
        }

        itemType =
            GetCollectionItemType(
                property.PropertyType,
                value.GetType());

        return list;
    }


    private static Type GetCollectionItemType(
        Type declaredType,
        Type runtimeType)
    {
        if (declaredType.IsArray)
        {
            return declaredType.GetElementType()
                   ?? throw new InvalidOperationException(
                       "Unable to determine array element type.");
        }

        if (declaredType.IsGenericType)
        {
            var args =
                declaredType.GetGenericArguments();

            if (args.Length == 1)
                return args[0];
        }

        if (runtimeType.IsGenericType)
        {
            var args =
                runtimeType.GetGenericArguments();

            if (args.Length == 1)
                return args[0];
        }

        var enumerableInterface =
            runtimeType
                .GetInterfaces()
                .FirstOrDefault(
                    x =>
                        x.IsGenericType &&
                        x.GetGenericTypeDefinition() ==
                        typeof(IEnumerable<>));

        if (enumerableInterface != null)
        {
            return enumerableInterface
                .GetGenericArguments()[0];
        }

        throw new InvalidOperationException(
            $"Unable to determine item type for collection '{runtimeType.Name}'.");
    }


    // ============================================================
    // OBJECT REPLACEMENT
    // ============================================================

    private static void ReplaceObjectProperty(
        BrochureExtractionDto model,
        string collection,
        string data,
        ImportAuditChange change)
    {
        var property =
            typeof(BrochureExtractionDto)
                .GetProperty(
                    collection,
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.IgnoreCase);

        if (property == null)
        {
            throw new InvalidOperationException(
                $"BrochureExtractionDto does not contain " +
                $"object '{collection}'.");
        }

        object? replacement;

        try
        {
            replacement =
                JsonSerializer.Deserialize(
                    data,
                    property.PropertyType,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' contains invalid " +
                $"replacement data for '{collection}'.",
                ex);
        }

        if (replacement == null)
        {
            throw new InvalidOperationException(
                $"Audit change '{change.AuditChangeId}' produced null " +
                $"replacement for '{collection}'.");
        }

        property.SetValue(
            model,
            replacement);
    }


    // ============================================================
    // COLLECTION NORMALIZATION
    // ============================================================

    private static string NormalizeCollectionName(
        string? collection)
    {
        if (string.IsNullOrWhiteSpace(collection))
            return string.Empty;

        return collection.Trim() switch
        {
            "powertrain" => "powertrains",
            "engine" => "engines",
            "transmission" => "transmissions",
            "drivetrain" => "drivetrains",
            "variant" => "variants",
            "feature" => "features",
            "specification" => "specifications",
            "fuelEfficiency" => "fuelEfficiencies",

            /*
             * Old specification collections are deliberately NOT
             * mapped to specifications.
             */
            "modelSpecifications" => "modelSpecifications",
            "powertrainSpecifications" => "powertrainSpecifications",
            "engineSpecifications" => "engineSpecifications",
            "transmissionSpecifications" => "transmissionSpecifications",
            "drivetrainSpecifications" => "drivetrainSpecifications",
            "variantSpecifications" => "variantSpecifications",

            var value => value
        };
    }


    private static bool IsSupportedCollection(
        string collection)
    {
        return collection switch
        {
            "document" => true,
            "vehicle" => true,

            "powertrains" => true,
            "engines" => true,
            "transmissions" => true,
            "drivetrains" => true,
            "variants" => true,
            "features" => true,
            "specifications" => true,
            "fuelEfficiencies" => true,
            "warnings" => true,

            /*
             * Explicitly reject old specification architecture.
             */
            "modelSpecifications" => false,
            "powertrainSpecifications" => false,
            "engineSpecifications" => false,
            "transmissionSpecifications" => false,
            "drivetrainSpecifications" => false,
            "variantSpecifications" => false,

            _ => false
        };
    }


    // ============================================================
    // DECISION NORMALIZATION
    // ============================================================

    private static string NormalizeDecision(
        string? decision)
    {
        if (string.IsNullOrWhiteSpace(decision))
        {
            throw new InvalidOperationException(
                "Audit decision is empty.");
        }

        if (string.Equals(
                decision,
                "Accepted",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Accepted";
        }

        if (string.Equals(
                decision,
                "Rejected",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Rejected";
        }

        if (string.Equals(
                decision,
                "Modified",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Modified";
        }

        if (string.Equals(
                decision,
                "Pending",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Pending";
        }

        throw new InvalidOperationException(
            $"Invalid audit decision '{decision}'.");
    }


    // ============================================================
    // MODIFIED DATA VALIDATION
    // ============================================================

    private static void ValidateModifiedDataJson(
        JsonElement? modifiedData,
        AuditChangeDto aiChange,
        string changeId)
    {
        if (!HasJsonData(modifiedData))
        {
            throw new InvalidOperationException(
                $"Audit change '{changeId}' ModifiedData is empty.");
        }

        /*
         * Remove actions should never be Modified.
         */
        if (string.Equals(
                aiChange.Action,
                "Remove",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Audit change '{changeId}' is Remove and cannot " +
                "contain ModifiedData.");
        }

        var data = modifiedData!.Value;

        if (data.ValueKind != JsonValueKind.Object &&
            data.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException(
                $"Audit change '{changeId}' ModifiedData must be a JSON " +
                "object (or a warning string).");
        }

        if (data.ValueKind == JsonValueKind.String)
        {
            if (!string.Equals(
                    aiChange.Target?.Collection,
                    "warnings",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Audit change '{changeId}' ModifiedData must be a JSON object.");
            }

            return;
        }

        if (!string.Equals(
                aiChange.Target?.Collection,
                "warnings",
                StringComparison.OrdinalIgnoreCase))
        {
            if (!data.TryGetProperty("indexId", out var indexIdElement) ||
                indexIdElement.ValueKind != JsonValueKind.Number ||
                !indexIdElement.TryGetInt32(out var dataIndexId) ||
                dataIndexId <= 0)
            {
                throw new InvalidOperationException(
                    $"Audit change '{changeId}' ModifiedData must contain " +
                    "a positive integer indexId.");
            }

            if (aiChange.Target?.IndexId is int targetIndexId &&
                dataIndexId != targetIndexId)
            {
                throw new InvalidOperationException(
                    $"Audit change '{changeId}' ModifiedData indexId " +
                    $"{dataIndexId} does not match target.indexId " +
                    $"{targetIndexId}.");
            }
        }
    }


    // ============================================================
    // STRING / REFLECTION HELPERS
    // ============================================================

    private static string? GetStringProperty(
        object item,
        string propertyName)
    {
        var property =
            item.GetType()
                .GetProperty(
                    propertyName,
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.IgnoreCase);

        if (property == null)
            return null;

        var value =
            property.GetValue(item);

        return value?.ToString();
    }


    private static void AddCandidate(
        string? value,
        out string? candidate)
    {
        candidate =
            string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
    }


    private static string DeserializeStringData(
        string data)
    {
        /*
         * Warnings can arrive either as a JSON string:
         *
         * "Some warning"
         *
         * or as plain text.
         */
        try
        {
            var parsed =
                JsonSerializer.Deserialize<string>(
                    data,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (parsed != null)
                return parsed;
        }
        catch (JsonException)
        {
            // Treat as raw string below.
        }

        return data.Trim();
    }


    // ============================================================
    // AUDIT CHANGE CONTAINER
    // ============================================================

    private sealed class AuditChangesContainerDto
    {
        public List<AuditChangeDto> Changes { get; set; } = new();
    }
}