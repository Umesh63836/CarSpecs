using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Powertrain
{
    public int PowertrainId { get; set; }

    public string PowertrainType { get; set; } = null!;

    public decimal? CombinedMaxPower { get; set; }

    public decimal? CombinedMaxTorque { get; set; }

    public int? CombinedMaxPowerRpm { get; set; }

    public int? CombinedMaxPowerRpmmin { get; set; }

    public int? CombinedMaxPowerRpmmax { get; set; }

    public int? CombinedMaxTorqueRpm { get; set; }

    public int? CombinedMaxTorqueRpmmin { get; set; }

    public int? CombinedMaxTorqueRpmmax { get; set; }

    public decimal? BatteryCapacityKwh { get; set; }

    public string? CombinedMaxPowerUnit { get; set; }

    public string? CombinedMaxTorqueUnit { get; set; }

    public virtual Engine? Engine { get; set; }

    public virtual ICollection<MotorPerformance> MotorPerformances { get; set; } = new List<MotorPerformance>();

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
