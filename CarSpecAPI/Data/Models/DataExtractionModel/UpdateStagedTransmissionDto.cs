namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class UpdateStagedTransmissionDto
    {
        public string? TransmissionType { get; set; }

        public int? NumberOfGears { get; set; }

        public bool? HasManualOverride { get; set; }

        public bool? HasPaddleShifters { get; set; }
    }
}
