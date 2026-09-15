using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Feature
{
    public int FeatureId { get; set; }

    public int FeatureCategoryId { get; set; }

    public string FeatureName { get; set; } = null!;

    public bool IsBaselineFeature { get; set; }

    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string? FeatureCode { get; set; }

    public string? ValueType { get; set; }

    public string? Unit { get; set; }

    public string? ExtractionGuidance { get; set; }

    public bool IsMultiValue { get; set; }

    public virtual FeatureCategory FeatureCategory { get; set; } = null!;

    public virtual ICollection<FeatureValueOption> FeatureValueOptions { get; set; } = new List<FeatureValueOption>();

    public virtual ICollection<ImportFeature> ImportFeatures { get; set; } = new List<ImportFeature>();

    public virtual ICollection<VariantFeature> VariantFeatures { get; set; } = new List<VariantFeature>();
}
