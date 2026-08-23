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

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
