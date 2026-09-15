using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class EngineLegacy20260909
{
    public int EngineId { get; set; }

    public string EngineName { get; set; } = null!;

    public int FuelTypeId { get; set; }

    public byte? NumberOfCylinders { get; set; }

    public byte? NumberOfValves { get; set; }

    public decimal? Displacement { get; set; }

    public bool IsTurbocharged { get; set; }

    public string? EmissionStandard { get; set; }

    public string? Aspiration { get; set; }

    public string? EngineType { get; set; }

    public decimal? BatteryCapacityKwh { get; set; }

    public decimal? CombinedMaxPower { get; set; }

    public decimal? CombinedMaxTorque { get; set; }

    public int? CombinedMaxPowerRpm { get; set; }

    public int? CombinedMaxPowerRpmmin { get; set; }

    public int? CombinedMaxPowerRpmmax { get; set; }

    public int? CombinedMaxTorqueRpm { get; set; }

    public int? CombinedMaxTorqueRpmmin { get; set; }

    public int? CombinedMaxTorqueRpmmax { get; set; }

    public virtual FuelType FuelType { get; set; } = null!;
}
