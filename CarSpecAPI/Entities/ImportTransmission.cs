using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ImportTransmission
{
    public int ImportTransmissionId { get; set; }

    public int ImportModelId { get; set; }

    public string TransmissionRef { get; set; } = null!;

    public string? TransmissionType { get; set; }

    public int? NumberOfGears { get; set; }

    public bool? HasManualOverride { get; set; }

    public bool? HasPaddleShifters { get; set; }

    public int? ProductionTransmissionId { get; set; }

    public virtual ImportModel ImportModel { get; set; } = null!;

    public virtual ICollection<ImportVariant> ImportVariants { get; set; } = new List<ImportVariant>();
}
