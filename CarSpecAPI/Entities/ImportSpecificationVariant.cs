using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportSpecificationVariant
{
    public int ImportSpecificationVariantId { get; set; }

    public int ImportSpecificationId { get; set; }

    public int ImportVariantId { get; set; }

    public decimal? NumericValue { get; set; }

    public string? TextValue { get; set; }

    public bool? BooleanValue { get; set; }

    public string? Unit { get; set; }

    public string? SourceColumn { get; set; }

    public int? PageNumber { get; set; }

    public string? Evidence { get; set; }

    public decimal? Confidence { get; set; }

    public virtual ImportSpecification ImportSpecification { get; set; } = null!;

    public virtual ImportVariant ImportVariant { get; set; } = null!;
}
