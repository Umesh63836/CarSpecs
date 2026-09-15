namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedPowertrainDto
    {
        public bool Exists { get; set; }

        public int? ImportPowertrainId { get; set; }

        public string? PowertrainRef { get; set; }

        public string? PowertrainType { get; set; }

        public string? EngineRef { get; set; }

        public decimal? BatteryCapacityKWh { get; set; }

        public decimal? CombinedMaxPower { get; set; }

        public decimal? CombinedMaxTorque { get; set; }

        public string? CombinedMaxPowerUnit { get; set; }

        public string? CombinedMaxTorqueUnit { get; set; }

        public int? CombinedMaxPowerRPM { get; set; }

        public int? CombinedMaxPowerRPMMin { get; set; }

        public int? CombinedMaxPowerRPMMax { get; set; }

        public int? CombinedMaxTorqueRPM { get; set; }

        public int? CombinedMaxTorqueRPMMin { get; set; }

        public int? CombinedMaxTorqueRPMMax { get; set; }

        public List<StagedMotorPerformanceDto> MotorPerformances { get; set; } = [];
    }
}
