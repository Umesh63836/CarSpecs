using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class VariantColour
{
    public int VariantColourId { get; set; }

    public int VariantId { get; set; }

    public int ColourId { get; set; }

    public bool IsAvailable { get; set; }

    public virtual Colour Colour { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}
