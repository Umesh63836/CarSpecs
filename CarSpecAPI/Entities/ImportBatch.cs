using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportBatch
{
    public int ImportBatchId { get; set; }

    public int DataSourceId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string Status { get; set; } = null!;

    public int? ImportedBy { get; set; }

    public string? Notes { get; set; }

    public string? Aimodel { get; set; }

    public DateTime? AiprocessedAt { get; set; }

    public string? AiresultJson { get; set; }

    public string? FinalJson { get; set; }

    public virtual DataSource DataSource { get; set; } = null!;

    public virtual ICollection<ImportAuditChange> ImportAuditChanges { get; set; } = new List<ImportAuditChange>();

    public virtual ICollection<ImportDocument> ImportDocuments { get; set; } = new List<ImportDocument>();

    public virtual ICollection<ImportRecord> ImportRecords { get; set; } = new List<ImportRecord>();

    public virtual ICollection<ImportWarning> ImportWarnings { get; set; } = new List<ImportWarning>();

    public virtual Admin? ImportedByNavigation { get; set; }
}
