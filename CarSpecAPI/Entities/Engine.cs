using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Engine
{
    public int EngineId { get; set; }

    public string EngineName { get; set; } = null!;

    public int FuelTypeId { get; set; }

    public byte? NumberOfCylinders { get; set; }

    public byte? NumberOfValves { get; set; }

    public decimal? Displacement { get; set; }

    public decimal? MaxPower { get; set; }

    public decimal? MaxTorque { get; set; }

    public bool IsTurbocharged { get; set; }

    public string? EmissionStandard { get; set; }

    public virtual FuelType FuelType { get; set; } = null!;

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
