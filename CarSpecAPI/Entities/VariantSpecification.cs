using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class VariantSpecification
{
    public int VariantSpecificationId { get; set; }

    public int VariantId { get; set; }

    public int SpecificationId { get; set; }

    public decimal? NumericValue { get; set; }

    public string? TextValue { get; set; }

    public bool? BooleanValue { get; set; }

    public string? SourceType { get; set; }

    public string? SourceUrl { get; set; }

    public bool IsActive { get; set; }

    public int? SourceId { get; set; }

    public string? Notes { get; set; }

    public virtual DataSource? Source { get; set; }

    public virtual Specification Specification { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}
