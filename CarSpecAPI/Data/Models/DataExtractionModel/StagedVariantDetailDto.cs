namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedVariantDetailDto
    {
        public int ImportVariantId { get; set; }

        public int ImportVariantParentId { get; set; }

        public string VariantName { get; set; } = string.Empty;

        public string VariantType { get; set; } = string.Empty;

        public string? BaseVariantName { get; set; }

        public decimal? ExShowroomPrice { get; set; }

        public int? KerbWeight { get; set; }

        public int? SeatingCapacity { get; set; }

        public string? PowertrainRef { get; set; }

        public string? TransmissionRef { get; set; }

        public string? DrivetrainRef { get; set; }

        public StagedPowertrainDto? Powertrain { get; set; } = new();

        public StagedTransmissionDto? Transmission { get; set; } = new();

        public StagedDrivetrainDto? Drivetrain { get; set; } = new();

        public StagedEngineDto? Engine { get; set; } = new();

        public List<StagedEnginePerformanceDto> EnginePerformances { get; set; } = [];

        public List<StagedMotorPerformanceDto> MotorPerformances { get; set; } = [];

        public List<StagedFeatureDto> Features { get; set; } = [];

        public List<StagedSpecificationDto> Specifications { get; set; } = [];

        public List<StagedFuelEfficiencyDto> FuelEfficiencies { get; set; } = [];

        public List<StagedWarrantyDto> Warranties { get; set; } = [];
    }
}
