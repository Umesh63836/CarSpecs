using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Warranty
{
    public int WarrantyId { get; set; }

    public int ModelId { get; set; }

    public int? VariantId { get; set; }

    public string WarrantyType { get; set; } = null!;

    public decimal? DurationYears { get; set; }

    public int? Kilometres { get; set; }

    public decimal? MaximumDurationYears { get; set; }

    public int? MaximumKilometres { get; set; }

    public int? SourceId { get; set; }

    public virtual Model Model { get; set; } = null!;

    public virtual Variant? Variant { get; set; }
}
