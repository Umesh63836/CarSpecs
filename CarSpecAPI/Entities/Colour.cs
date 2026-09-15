using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Colour
{
    public int ColourId { get; set; }

    public string ColourName { get; set; } = null!;

    public string? HexCode { get; set; }

    public string? ColourType { get; set; }

    public virtual ICollection<VariantColour> VariantColours { get; set; } = new List<VariantColour>();
}
