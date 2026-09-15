using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class FuelType
{
    public int FuelTypeId { get; set; }

    public string FuelType1 { get; set; } = null!;

    public virtual ICollection<EngineLegacy20260909> EngineLegacy20260909s { get; set; } = new List<EngineLegacy20260909>();

    public virtual ICollection<EnginePerformance> EnginePerformances { get; set; } = new List<EnginePerformance>();

    public virtual ICollection<ImportEnginePerformance> ImportEnginePerformances { get; set; } = new List<ImportEnginePerformance>();
}
