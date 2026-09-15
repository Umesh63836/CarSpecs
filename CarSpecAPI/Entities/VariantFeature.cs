using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class VariantFeature
{
    public int VariantFeatureId { get; set; }

    public int VariantId { get; set; }

    public int FeatureId { get; set; }

    public bool IsAvailable { get; set; }

    public string? FeatureValue { get; set; }

    public string? SourceType { get; set; }

    public string? SourceUrl { get; set; }

    public bool IsActive { get; set; }

    public decimal? NumericValue { get; set; }

    public string? Unit { get; set; }

    public int? SourceId { get; set; }

    public string? Notes { get; set; }

    public virtual Feature Feature { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}
