namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class DrivetrainDto
    {
        public int IndexId { get; set; }
        public string DrivetrainRef { get; set; } = null!;
        public string? DrivetrainType { get; set; }
        public string? DifferentialType { get; set; }
    }
}
