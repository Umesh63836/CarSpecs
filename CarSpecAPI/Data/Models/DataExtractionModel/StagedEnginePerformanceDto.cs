namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedEnginePerformanceDto
    {
        public int ImportEnginePerformanceId { get; set; }

        public string ModeName { get; set; } = string.Empty;

        public string? FuelTypeName { get; set; }

        public decimal? MaxPower { get; set; }

        public string? MaxPowerUnit { get; set; }

        public decimal? MaxTorque { get; set; }

        public string? MaxTorqueUnit { get; set; }

        public int? MaxPowerRPM { get; set; }

        public int? MaxPowerRPMMin { get; set; }

        public int? MaxPowerRPMMax { get; set; }

        public int? MaxTorqueRPM { get; set; }

        public int? MaxTorqueRPMMin { get; set; }

        public int? MaxTorqueRPMMax { get; set; }
    }
}
