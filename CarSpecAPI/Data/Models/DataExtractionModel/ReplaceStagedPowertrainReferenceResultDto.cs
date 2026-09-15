namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class ReplaceStagedPowertrainReferenceResultDto
    {
        public int ImportVariantId { get; set; }
        public int? PreviousImportPowertrainId { get; set; }
        public int NewImportPowertrainId { get; set; }
        public string? PreviousPowertrainRef { get; set; }
        public string NewPowertrainRef { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
