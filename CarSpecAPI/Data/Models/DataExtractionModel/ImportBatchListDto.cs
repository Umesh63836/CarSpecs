namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public class ImportBatchListDto
    {
        public int ImportBatchId { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string Status { get; set; } = string.Empty;

        public int? ImportedBy { get; set; }

        public string? Notes { get; set; }

        public string? AIModel { get; set; }

        public DateTime? AIProcessedAt { get; set; }

        public bool HasAIResult { get; set; }

        public bool HasFinalResult { get; set; }

        public List<ImportBatchDocumentDto> Documents { get; set; } = new();
    }
}
