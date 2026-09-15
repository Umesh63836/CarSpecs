using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class EnginePerformance
{
    public int EnginePerformanceId { get; set; }

    public int EngineId { get; set; }

    public string ModeName { get; set; } = null!;

    public int? FuelTypeId { get; set; }

    public decimal? MaxPower { get; set; }

    public decimal? MaxTorque { get; set; }

    public int? MaxPowerRpm { get; set; }

    public int? MaxPowerRpmmin { get; set; }

    public int? MaxPowerRpmmax { get; set; }

    public int? MaxTorqueRpm { get; set; }

    public int? MaxTorqueRpmmin { get; set; }

    public int? MaxTorqueRpmmax { get; set; }

    public string? MaxPowerUnit { get; set; }

    public string? MaxTorqueUnit { get; set; }

    public virtual Engine Engine { get; set; } = null!;

    public virtual FuelType? FuelType { get; set; }
}
