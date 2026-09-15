namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StagedWarningDto
    {
        public int ImportWarningId { get; set; }

        public int? ImportRecordId { get; set; }

        public string WarningText { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int? ReviewedBy { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public string? Notes { get; set; }
    }
}
