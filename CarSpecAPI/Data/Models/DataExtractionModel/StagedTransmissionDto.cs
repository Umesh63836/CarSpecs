namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedTransmissionDto
    {
        public bool Exists { get; set; }

        public int? ImportTransmissionId { get; set; }

        public string? TransmissionRef { get; set; }

        public string? TransmissionType { get; set; }

        public int? NumberOfGears { get; set; }

        public bool? HasManualOverride { get; set; }

        public bool? HasPaddleShifters { get; set; }
    }
}
