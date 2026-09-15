using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportFeature
{
    public int ImportFeatureId { get; set; }

    public int? FeatureId { get; set; }

    public string FeatureCode { get; set; } = null!;

    public virtual Feature? Feature { get; set; }

    public virtual ICollection<ImportFeatureVariant> ImportFeatureVariants { get; set; } = new List<ImportFeatureVariant>();
}
