using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportEnginePerformance
{
    public int ImportEnginePerformanceId { get; set; }

    public int ImportEngineId { get; set; }

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

    public int? ProductionEnginePerformanceId { get; set; }

    public string? FuelTypeName { get; set; }

    public string? MaxPowerUnit { get; set; }

    public string? MaxTorqueUnit { get; set; }

    public virtual FuelType? FuelType { get; set; }

    public virtual ImportEngine ImportEngine { get; set; } = null!;
}
