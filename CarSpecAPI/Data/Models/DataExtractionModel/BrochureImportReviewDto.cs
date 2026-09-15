namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public class BrochureImportReviewDto
    {
        public int ImportBatchId { get; set; }

        public string Status { get; set; } = string.Empty;

        public BrochureExtractionDto Model1 { get; set; } = new();

        public List<AuditChangeReviewDto> AuditChanges { get; set; } = new();

        public List<string> Warnings { get; set; } = new();
    }
}
