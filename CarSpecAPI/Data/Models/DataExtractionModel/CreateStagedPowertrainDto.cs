namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class CreateStagedPowertrainDto
    {
        public int ImportModelId { get; set; }

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


        // ENGINE

        public string EngineMode { get; set; } = "none";

        public int? ExistingEngineId { get; set; }

        public CreateStagedEngineDto? NewEngine { get; set; }


        // ENGINE PERFORMANCE

        public string EnginePerformanceMode { get; set; } = "none";

        public int? ExistingEnginePerformanceId { get; set; }

        public CreateStagedEnginePerformanceDto? NewEnginePerformance { get; set; }


        // MOTOR PERFORMANCE

        public List<int> ExistingMotorPerformanceIds { get; set; } = [];

        public List<CreateStagedMotorPerformanceDto> NewMotorPerformances { get; set; } = [];
    }
}
