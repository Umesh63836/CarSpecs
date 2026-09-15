using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class SpecificationCategory
{
    public int SpecificationCategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Specification> Specifications { get; set; } = new List<Specification>();
}
