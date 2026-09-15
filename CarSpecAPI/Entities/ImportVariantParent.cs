using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportVariantParent
{
    public int ImportVariantParentId { get; set; }

    public int ImportModelId { get; set; }

    public string ParentVariantName { get; set; } = null!;

    public virtual ImportModel ImportModel { get; set; } = null!;

    public virtual ICollection<ImportVariant> ImportVariants { get; set; } = new List<ImportVariant>();
}
