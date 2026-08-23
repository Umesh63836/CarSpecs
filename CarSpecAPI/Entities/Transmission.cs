using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Transmission
{
    public int TransmissionId { get; set; }

    public string TransmissionType { get; set; } = null!;

    public byte? NumberOfGears { get; set; }

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
