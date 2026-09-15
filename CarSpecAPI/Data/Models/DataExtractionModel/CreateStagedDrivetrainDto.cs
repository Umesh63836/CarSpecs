namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class CreateStagedDrivetrainDto
    {
        public int ImportModelId { get; set; }

        public string DrivetrainRef { get; set; } = string.Empty;

        public string? DrivetrainType { get; set; }

        public string? DifferentialType { get; set; }
    }
}
