using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Variant
{
    public int VariantId { get; set; }

    public int ModelId { get; set; }

    public string VariantName { get; set; } = null!;

    public int TransmissionId { get; set; }

    public int DrivetrainId { get; set; }

    public decimal? ExShowroomPrice { get; set; }

    public int? KerbWeight { get; set; }

    public int? SeatingCapacity { get; set; }

    public int PowertrainId { get; set; }

    public virtual Drivetrain Drivetrain { get; set; } = null!;

    public virtual Model Model { get; set; } = null!;

    public virtual Powertrain Powertrain { get; set; } = null!;

    public virtual Transmission Transmission { get; set; } = null!;

    public virtual ICollection<VariantColour> VariantColours { get; set; } = new List<VariantColour>();

    public virtual ICollection<VariantFeature> VariantFeatures { get; set; } = new List<VariantFeature>();

    public virtual ICollection<VariantFuelEfficiency> VariantFuelEfficiencies { get; set; } = new List<VariantFuelEfficiency>();

    public virtual ICollection<VariantImage> VariantImages { get; set; } = new List<VariantImage>();

    public virtual ICollection<VariantSpecification> VariantSpecifications { get; set; } = new List<VariantSpecification>();

    public virtual ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
}
