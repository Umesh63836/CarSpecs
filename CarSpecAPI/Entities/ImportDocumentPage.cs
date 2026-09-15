using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportDocumentPage
{
    public int ImportDocumentPageId { get; set; }

    public int ImportDocumentId { get; set; }

    public int PageNumber { get; set; }

    public string? ExtractedText { get; set; }

    public string? PagePdfPath { get; set; }

    public string? PageImagePath { get; set; }

    public bool IsSelected { get; set; }

    public string? ProcessingStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ImportDocument ImportDocument { get; set; } = null!;
}
