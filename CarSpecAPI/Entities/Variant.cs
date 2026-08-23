using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Variant
{
    public int VariantId { get; set; }

    public int ModelId { get; set; }

    public string VariantName { get; set; } = null!;

    public int EngineId { get; set; }

    public int TransmissionId { get; set; }

    public int DrivetrainId { get; set; }

    public decimal? ExShowroomPrice { get; set; }

    public virtual Drivetrain Drivetrain { get; set; } = null!;

    public virtual Engine Engine { get; set; } = null!;

    public virtual Model Model { get; set; } = null!;

    public virtual Transmission Transmission { get; set; } = null!;

    public virtual ICollection<VariantImage> VariantImages { get; set; } = new List<VariantImage>();
}
