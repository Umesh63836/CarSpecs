using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class FeatureCategory
{
    public int FeatureCategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Feature> Features { get; set; } = new List<Feature>();
}
