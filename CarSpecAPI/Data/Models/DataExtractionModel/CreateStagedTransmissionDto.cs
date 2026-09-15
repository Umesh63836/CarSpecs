namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class CreateStagedTransmissionDto
    {
        public int ImportModelId { get; set; }

        public string TransmissionRef { get; set; } = string.Empty;

        public string? TransmissionType { get; set; }

        public int? NumberOfGears { get; set; }

        public bool? HasManualOverride { get; set; }

        public bool? HasPaddleShifters { get; set; }
    }
}
