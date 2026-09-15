using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportSpecification
{
    public int ImportSpecificationId { get; set; }

    public int? SpecificationId { get; set; }

    public string SpecificationCode { get; set; } = null!;

    public virtual ICollection<ImportSpecificationVariant> ImportSpecificationVariants { get; set; } = new List<ImportSpecificationVariant>();

    public virtual Specification? Specification { get; set; }
}
