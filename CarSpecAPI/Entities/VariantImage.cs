using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class VariantImage
{
    public int VariantImageId { get; set; }

    public int VariantId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public string? ImageType { get; set; }

    public int? DisplayOrder { get; set; }

    public virtual Variant Variant { get; set; } = null!;
}
