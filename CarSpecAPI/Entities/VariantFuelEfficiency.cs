using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class VariantFuelEfficiency
{
    public int VariantFuelEfficiencyId { get; set; }

    public int VariantId { get; set; }

    public decimal FuelEfficiency { get; set; }

    public string FuelEfficiencyUnit { get; set; } = null!;

    public string SourceType { get; set; } = null!;

    public string? SourceUrl { get; set; }

    public bool IsActive { get; set; }

    public short? TestYear { get; set; }

    public int? SourceId { get; set; }

    public virtual Variant Variant { get; set; } = null!;
}
