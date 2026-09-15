using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportRecord
{
    public int ImportRecordId { get; set; }

    public int ImportBatchId { get; set; }

    public int? ImportDocumentId { get; set; }

    public string EntityType { get; set; } = null!;

    public string? EntityKey { get; set; }

    public string? FieldName { get; set; }

    public string? ExtractedValue { get; set; }

    public string? NormalizedValue { get; set; }

    public decimal? Confidence { get; set; }

    public int? PageNumber { get; set; }

    public string? EvidenceText { get; set; }

    public string? Status { get; set; }

    public int? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? Notes { get; set; }

    public decimal? NumericValue { get; set; }

    public string? Unit { get; set; }

    public int? SourceId { get; set; }

    public int? ParentImportRecordId { get; set; }

    public string? AuditChangeId { get; set; }

    public string? ReviewStatus { get; set; }

    public virtual ICollection<ImportAuditChange> ImportAuditChanges { get; set; } = new List<ImportAuditChange>();

    public virtual ImportBatch ImportBatch { get; set; } = null!;

    public virtual ImportDocument? ImportDocument { get; set; }

    public virtual ImportModel? ImportModel { get; set; }

    public virtual ICollection<ImportWarning> ImportWarnings { get; set; } = new List<ImportWarning>();

    public virtual ICollection<ImportRecord> InverseParentImportRecord { get; set; } = new List<ImportRecord>();

    public virtual ImportRecord? ParentImportRecord { get; set; }

    public virtual Admin? ReviewedByNavigation { get; set; }

    public virtual DataSource? Source { get; set; }
}
