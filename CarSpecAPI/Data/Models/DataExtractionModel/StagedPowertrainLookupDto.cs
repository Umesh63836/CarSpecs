namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedPowertrainLookupDto
    {
        public int ImportPowertrainId { get; set; }
        public int ImportModelId { get; set; }

        public string PowertrainRef { get; set; } = null!;
        public string PowertrainType { get; set; } = null!;

        public string? EngineRef { get; set; }

        public decimal? BatteryCapacityKWh { get; set; }

        public decimal? CombinedMaxPower { get; set; }
        public string? CombinedMaxPowerUnit { get; set; }

        public decimal? CombinedMaxTorque { get; set; }
        public string? CombinedMaxTorqueUnit { get; set; }

        public int? CombinedMaxPowerRPM { get; set; }
        public int? CombinedMaxPowerRPMMin { get; set; }
        public int? CombinedMaxPowerRPMMax { get; set; }

        public int? CombinedMaxTorqueRPM { get; set; }
        public int? CombinedMaxTorqueRPMMin { get; set; }
        public int? CombinedMaxTorqueRPMMax { get; set; }

        public int? ProductionPowertrainId { get; set; }
    }
}
