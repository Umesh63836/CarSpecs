using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class VariantLegacyBackup20260909
{
    public int VariantId { get; set; }

    public int ModelId { get; set; }

    public string VariantName { get; set; } = null!;

    public int EngineId { get; set; }

    public int TransmissionId { get; set; }

    public int DrivetrainId { get; set; }

    public decimal? ExShowroomPrice { get; set; }

    public int? KerbWeight { get; set; }

    public int? SeatingCapacity { get; set; }
}
