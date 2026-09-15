using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportWarranty
{
    public int ImportWarrantyId { get; set; }

    public int ImportModelId { get; set; }

    public int? ImportVariantId { get; set; }

    public string WarrantyType { get; set; } = null!;

    public decimal? DurationYears { get; set; }

    public int? Kilometres { get; set; }

    public decimal? MaximumDurationYears { get; set; }

    public int? MaximumKilometres { get; set; }

    public int? SourceId { get; set; }

    public string? EvidenceText { get; set; }

    public int? PageNumber { get; set; }

    public decimal? Confidence { get; set; }

    public virtual ImportModel ImportModel { get; set; } = null!;

    public virtual ImportVariant? ImportVariant { get; set; }
}
