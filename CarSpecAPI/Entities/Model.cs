using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Model
{
    public int ModelId { get; set; }

    public string ModelName { get; set; } = null!;

    public int BrandId { get; set; }

    public bool IsActive { get; set; }

    public short? DiscontinuedYear { get; set; }

    public short? LaunchYear { get; set; }

    public string? ModelImageUrl { get; set; }

    public string? Category { get; set; }

    public string? BodyType { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ModelDimension? ModelDimension { get; set; }

    public virtual ICollection<ModelSafetyRating> ModelSafetyRatings { get; set; } = new List<ModelSafetyRating>();

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();

    public virtual ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
}
