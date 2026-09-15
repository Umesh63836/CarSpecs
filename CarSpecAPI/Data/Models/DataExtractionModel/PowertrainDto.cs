namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class PowertrainDto
    {
        public int IndexId { get; set; }
        public string PowertrainRef { get; set; } = null!;
        public string PowertrainType { get; set; } = null!;

        public string? EngineRef { get; set; }

        public decimal? BatteryCapacityKWh { get; set; }

        public decimal? CombinedMaxPower { get; set; }
        public decimal? CombinedMaxTorque { get; set; }

        public int? CombinedMaxPowerRPM { get; set; }
        public int? CombinedMaxPowerRPMMin { get; set; }
        public int? CombinedMaxPowerRPMMax { get; set; }

        public int? CombinedMaxTorqueRPM { get; set; }
        public int? CombinedMaxTorqueRPMMin { get; set; }
        public int? CombinedMaxTorqueRPMMax { get; set; }

        public List<MotorPerformanceDto> MotorPerformances { get; set; } = [];
    }
}
