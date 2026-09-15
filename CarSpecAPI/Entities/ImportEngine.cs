using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportEngine
{
    public int ImportEngineId { get; set; }

    public int ImportPowertrainId { get; set; }

    public string EngineRef { get; set; } = null!;

    public string EngineName { get; set; } = null!;

    public byte? NumberOfCylinders { get; set; }

    public byte? NumberOfValves { get; set; }

    public decimal? Displacement { get; set; }

    public bool? IsTurbocharged { get; set; }

    public string? EmissionStandard { get; set; }

    public string? Aspiration { get; set; }

    public string? EngineType { get; set; }

    public int? ProductionEngineId { get; set; }

    public virtual ICollection<ImportEnginePerformance> ImportEnginePerformances { get; set; } = new List<ImportEnginePerformance>();

    public virtual ImportPowertrain ImportPowertrain { get; set; } = null!;
}
