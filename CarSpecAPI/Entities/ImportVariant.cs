using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportVariant
{
    public int ImportVariantId { get; set; }

    public int ImportVariantParentId { get; set; }

    public string VariantName { get; set; } = null!;

    public string VariantType { get; set; } = null!;

    public string? BaseVariantName { get; set; }

    public string? PowertrainRef { get; set; }

    public string? TransmissionRef { get; set; }

    public string? DrivetrainRef { get; set; }

    public decimal? ExShowroomPrice { get; set; }

    public int? KerbWeight { get; set; }

    public int? SeatingCapacity { get; set; }

    public int? ProductionVariantId { get; set; }

    public int? ImportPowertrainId { get; set; }

    public int? ImportTransmissionId { get; set; }

    public int? ImportDrivetrainId { get; set; }

    public virtual ImportDrivetrain? ImportDrivetrain { get; set; }

    public virtual ICollection<ImportFeatureVariant> ImportFeatureVariants { get; set; } = new List<ImportFeatureVariant>();

    public virtual ICollection<ImportFuelEfficiencyVariant> ImportFuelEfficiencyVariants { get; set; } = new List<ImportFuelEfficiencyVariant>();

    public virtual ImportPowertrain? ImportPowertrain { get; set; }

    public virtual ICollection<ImportSpecificationVariant> ImportSpecificationVariants { get; set; } = new List<ImportSpecificationVariant>();

    public virtual ImportTransmission? ImportTransmission { get; set; }

    public virtual ImportVariantParent ImportVariantParent { get; set; } = null!;

    public virtual ICollection<ImportWarranty> ImportWarranties { get; set; } = new List<ImportWarranty>();
}
