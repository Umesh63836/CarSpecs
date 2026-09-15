namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class ReplaceStagedDrivetrainReferenceResultDto
    {
        public int ImportVariantId { get; set; }

        public int? PreviousImportDrivetrainId { get; set; }
        public int NewImportDrivetrainId { get; set; }

        public string? PreviousDrivetrainRef { get; set; }
        public string NewDrivetrainRef { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
