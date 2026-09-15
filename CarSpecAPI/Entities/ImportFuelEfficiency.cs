using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportFuelEfficiency
{
    public int ImportFuelEfficiencyId { get; set; }

    public decimal? FuelEfficiency { get; set; }

    public string? FuelEfficiencyUnit { get; set; }

    public string? SourceColumn { get; set; }

    public int? PageNumber { get; set; }

    public string? Evidence { get; set; }

    public decimal? Confidence { get; set; }

    public int? ProductionFuelEfficiencyId { get; set; }

    public string? SourceType { get; set; }

    public bool IsActive { get; set; }

    public int? SourceId { get; set; }

    public virtual ICollection<ImportFuelEfficiencyVariant> ImportFuelEfficiencyVariants { get; set; } = new List<ImportFuelEfficiencyVariant>();

    public virtual DataSource? Source { get; set; }
}
