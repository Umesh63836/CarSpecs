namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public class SaveAdminReviewRequest
    {
        public int? ReviewedBy { get; set; }
        public List<AdminAuditDecisionDto> Decisions { get; set; }
            = new();
    }
}
