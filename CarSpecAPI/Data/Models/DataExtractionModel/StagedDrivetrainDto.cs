namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedDrivetrainDto
    {
        public bool Exists { get; set; }

        public int? ImportDrivetrainId { get; set; }

        public string? DrivetrainRef { get; set; }

        public string? DrivetrainType { get; set; }

        public string? DifferentialType { get; set; }
    }
}
