namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class CreateStagedEnginePerformanceDto
    {
        public string ModeName { get; set; } = null!;

        public int? FuelTypeId { get; set; }

        public decimal? MaxPower { get; set; }
        public decimal? MaxTorque { get; set; }

        public int? MaxPowerRPM { get; set; }
        public int? MaxPowerRPMMin { get; set; }
        public int? MaxPowerRPMMax { get; set; }

        public int? MaxTorqueRPM { get; set; }
        public int? MaxTorqueRPMMin { get; set; }
        public int? MaxTorqueRPMMax { get; set; }
    }
}
