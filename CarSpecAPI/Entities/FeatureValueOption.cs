using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class FeatureValueOption
{
    public int FeatureValueOptionId { get; set; }

    public int FeatureId { get; set; }

    public string Value { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual Feature Feature { get; set; } = null!;

    public virtual ICollection<ImportFeatureValueOption> ImportFeatureValueOptions { get; set; } = new List<ImportFeatureValueOption>();
}
