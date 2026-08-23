using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Drivetrain
{
    public int DrivetrainId { get; set; }

    public string DrivetrainType { get; set; } = null!;

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
