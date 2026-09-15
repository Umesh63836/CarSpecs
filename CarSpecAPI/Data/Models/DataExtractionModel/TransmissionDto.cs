namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class TransmissionDto
    {
        public int IndexId { get; set; }
        public string TransmissionRef { get; set; } = null!;
        public string? TransmissionType { get; set; }
        public int? NumberOfGears { get; set; }
        public bool? HasManualOverride { get; set; }
        public bool? HasPaddleShifters { get; set; }
    }
}
