using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Specification
{
    public int SpecificationId { get; set; }

    public int SpecificationCategoryId { get; set; }

    public string SpecificationName { get; set; } = null!;

    public string DataType { get; set; } = null!;

    public string? Unit { get; set; }

    public bool IsFilterable { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string? SpecificationCode { get; set; }

    public string? Description { get; set; }

    public string? ExtractionGuidance { get; set; }

    public virtual ICollection<ImportSpecification> ImportSpecifications { get; set; } = new List<ImportSpecification>();

    public virtual SpecificationCategory SpecificationCategory { get; set; } = null!;

    public virtual ICollection<VariantSpecification> VariantSpecifications { get; set; } = new List<VariantSpecification>();
}
