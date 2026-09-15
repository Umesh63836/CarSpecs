namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class BrochureUploadResponse
    {
        public int DataSourceId { get; set; }

        public int ImportBatchId { get; set; }

        public int ImportDocumentId { get; set; }

        public string FileName { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
