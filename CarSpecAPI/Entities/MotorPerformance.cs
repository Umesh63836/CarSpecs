using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class MotorPerformance
{
    public int MotorPerformanceId { get; set; }

    public int PowertrainId { get; set; }

    public string? MotorName { get; set; }

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

    public virtual Powertrain Powertrain { get; set; } = null!;
}
