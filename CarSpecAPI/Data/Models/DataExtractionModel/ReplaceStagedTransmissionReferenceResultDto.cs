namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class ReplaceStagedTransmissionReferenceResultDto
    {
        public int ImportVariantId { get; set; }

        public int? PreviousImportTransmissionId { get; set; }
        public int NewImportTransmissionId { get; set; }

        public string? PreviousTransmissionRef { get; set; }
        public string NewTransmissionRef { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
