using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportPowertrain
{
    public int ImportPowertrainId { get; set; }

    public int ImportModelId { get; set; }

    public string PowertrainRef { get; set; } = null!;

    public string PowertrainType { get; set; } = null!;

    public string? EngineRef { get; set; }

    public decimal? BatteryCapacityKwh { get; set; }

    public decimal? CombinedMaxPower { get; set; }

    public decimal? CombinedMaxTorque { get; set; }

    public int? CombinedMaxPowerRpm { get; set; }

    public int? CombinedMaxPowerRpmmin { get; set; }

    public int? CombinedMaxPowerRpmmax { get; set; }

    public int? CombinedMaxTorqueRpm { get; set; }

    public int? CombinedMaxTorqueRpmmin { get; set; }

    public int? CombinedMaxTorqueRpmmax { get; set; }

    public int? ProductionPowertrainId { get; set; }

    public string? CombinedMaxPowerUnit { get; set; }

    public string? CombinedMaxTorqueUnit { get; set; }

    public virtual ImportEngine? ImportEngine { get; set; }

    public virtual ImportModel ImportModel { get; set; } = null!;

    public virtual ICollection<ImportMotorPerformance> ImportMotorPerformances { get; set; } = new List<ImportMotorPerformance>();

    public virtual ICollection<ImportVariant> ImportVariants { get; set; } = new List<ImportVariant>();
}
