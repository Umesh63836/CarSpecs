using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportFeatureVariant
{
    public int ImportFeatureVariantId { get; set; }

    public int ImportFeatureId { get; set; }

    public int ImportVariantId { get; set; }

    public bool? Available { get; set; }

    public string? Value { get; set; }

    public string? SourceColumn { get; set; }

    public int? PageNumber { get; set; }

    public string? Evidence { get; set; }

    public decimal? Confidence { get; set; }

    public virtual ImportFeature ImportFeature { get; set; } = null!;

    public virtual ICollection<ImportFeatureValueOption> ImportFeatureValueOptions { get; set; } = new List<ImportFeatureValueOption>();

    public virtual ImportVariant ImportVariant { get; set; } = null!;
}
