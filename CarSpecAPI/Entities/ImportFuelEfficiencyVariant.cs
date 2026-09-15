using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportFuelEfficiencyVariant
{
    public int ImportFuelEfficiencyVariantId { get; set; }

    public int ImportFuelEfficiencyId { get; set; }

    public int ImportVariantId { get; set; }

    public virtual ImportFuelEfficiency ImportFuelEfficiency { get; set; } = null!;

    public virtual ImportVariant ImportVariant { get; set; } = null!;
}
