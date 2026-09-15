using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class State
{
    public short StateCode { get; set; }

    public int? StateVersion { get; set; }

    public string StateName { get; set; } = null!;

    public string StateOrUt { get; set; } = null!;

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
