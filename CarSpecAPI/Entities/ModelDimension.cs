using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ModelDimension
{
    public int ModelDimensionId { get; set; }

    public int ModelId { get; set; }

    public decimal? LengthMm { get; set; }

    public decimal? WidthMm { get; set; }

    public decimal? HeightMm { get; set; }

    public decimal? WheelbaseMm { get; set; }

    public decimal? GroundClearanceMm { get; set; }

    public decimal? BootSpaceLitres { get; set; }

    public decimal? FuelTankCapacityLitres { get; set; }

    public virtual Model Model { get; set; } = null!;
}
