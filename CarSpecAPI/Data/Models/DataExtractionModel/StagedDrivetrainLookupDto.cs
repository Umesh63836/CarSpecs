namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedDrivetrainLookupDto
    {
        public int ImportDrivetrainId { get; set; }
        public int ImportModelId { get; set; }

        public string DrivetrainRef { get; set; } = null!;
        public string? DrivetrainType { get; set; }
        public string? DifferentialType { get; set; }

        public int? ProductionDrivetrainId { get; set; }
    }
}
