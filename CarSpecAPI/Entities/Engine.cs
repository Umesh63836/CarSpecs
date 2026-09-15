using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Engine
{
    public int EngineId { get; set; }

    public int PowertrainId { get; set; }

    public string EngineName { get; set; } = null!;

    public byte? NumberOfCylinders { get; set; }

    public byte? NumberOfValves { get; set; }

    public decimal? Displacement { get; set; }

    public bool IsTurbocharged { get; set; }

    public string? EmissionStandard { get; set; }

    public string? Aspiration { get; set; }

    public string? EngineType { get; set; }

    public virtual ICollection<EnginePerformance> EnginePerformances { get; set; } = new List<EnginePerformance>();

    public virtual Powertrain Powertrain { get; set; } = null!;
}
