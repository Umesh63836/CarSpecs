using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportWarning
{
    public int ImportWarningId { get; set; }

    public int ImportBatchId { get; set; }

    public int? ImportRecordId { get; set; }

    public string WarningText { get; set; } = null!;

    public string? Status { get; set; }

    public int? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? Notes { get; set; }

    public virtual ImportBatch ImportBatch { get; set; } = null!;

    public virtual ImportRecord? ImportRecord { get; set; }

    public virtual Admin? ReviewedByNavigation { get; set; }
}
