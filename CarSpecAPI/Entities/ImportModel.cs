using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportModel
{
    public int ImportModelId { get; set; }

    public int ImportRecordId { get; set; }

    public string? BrandName { get; set; }

    public string ModelName { get; set; } = null!;

    public string? Category { get; set; }

    public string? BodyType { get; set; }

    public int? ProductionBrandId { get; set; }

    public int? ProductionModelId { get; set; }

    public string? ModelImageUrl { get; set; }

    public virtual ICollection<ImportDrivetrain> ImportDrivetrains { get; set; } = new List<ImportDrivetrain>();

    public virtual ImportModelDimension? ImportModelDimension { get; set; }

    public virtual ICollection<ImportPowertrain> ImportPowertrains { get; set; } = new List<ImportPowertrain>();

    public virtual ImportRecord ImportRecord { get; set; } = null!;

    public virtual ICollection<ImportTransmission> ImportTransmissions { get; set; } = new List<ImportTransmission>();

    public virtual ICollection<ImportVariantParent> ImportVariantParents { get; set; } = new List<ImportVariantParent>();

    public virtual ICollection<ImportWarranty> ImportWarranties { get; set; } = new List<ImportWarranty>();
}
