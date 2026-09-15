using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportAuditChange
{
    public int ImportAuditChangeId { get; set; }

    public int ImportBatchId { get; set; }

    public int? TargetImportRecordId { get; set; }

    public string? AuditChangeId { get; set; }

    public string Action { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public string? EntityKey { get; set; }

    public string? FieldName { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public int? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? Notes { get; set; }

    public virtual ImportBatch ImportBatch { get; set; } = null!;

    public virtual Admin? ReviewedByNavigation { get; set; }

    public virtual ImportRecord? TargetImportRecord { get; set; }
}
