namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public class ImportBatchDocumentDto
    {
        public int ImportDocumentId { get; set; }

        public string DocumentType { get; set; } = string.Empty;

        public string DocumentName { get; set; } = string.Empty;

        public long? FileSizeBytes { get; set; }
    }
}
