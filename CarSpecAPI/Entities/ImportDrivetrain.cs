using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportDrivetrain
{
    public int ImportDrivetrainId { get; set; }

    public int ImportModelId { get; set; }

    public string DrivetrainRef { get; set; } = null!;

    public string? DrivetrainType { get; set; }

    public string? DifferentialType { get; set; }

    public int? ProductionDrivetrainId { get; set; }

    public virtual ImportModel ImportModel { get; set; } = null!;

    public virtual ICollection<ImportVariant> ImportVariants { get; set; } = new List<ImportVariant>();
}
