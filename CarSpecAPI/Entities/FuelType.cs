using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class FuelType
{
    public int FuelTypeId { get; set; }

    public string FuelType1 { get; set; } = null!;

    public virtual ICollection<Engine> Engines { get; set; } = new List<Engine>();
}
