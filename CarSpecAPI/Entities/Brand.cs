using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public string? LogoUrl { get; set; }

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
