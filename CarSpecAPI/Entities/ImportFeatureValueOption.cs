using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportFeatureValueOption
{
    public int ImportFeatureVariantId { get; set; }

    public int ValueOptionId { get; set; }

    public int ImportFeatureValueOptionId { get; set; }

    public virtual ImportFeatureVariant ImportFeatureVariant { get; set; } = null!;

    public virtual FeatureValueOption ValueOption { get; set; } = null!;
}
