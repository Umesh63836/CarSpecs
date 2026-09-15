using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportDocument
{
    public int ImportDocumentId { get; set; }

    public int ImportBatchId { get; set; }

    public string DocumentType { get; set; } = null!;

    public string DocumentName { get; set; } = null!;

    public string? SourceUrl { get; set; }

    public string? LocalFilePath { get; set; }

    public string? ContentHash { get; set; }

    public string? ExtractedText { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? BlobContainer { get; set; }

    public string? BlobPath { get; set; }

    public long? FileSizeBytes { get; set; }

    public string? MimeType { get; set; }

    public int? PageCount { get; set; }

    public string? ExtractionStatus { get; set; }

    public virtual ImportBatch ImportBatch { get; set; } = null!;

    public virtual ICollection<ImportDocumentPage> ImportDocumentPages { get; set; } = new List<ImportDocumentPage>();

    public virtual ICollection<ImportRecord> ImportRecords { get; set; } = new List<ImportRecord>();
}
