namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public class AuditChangeTargetDto
    {
        public string Collection { get; set; } = string.Empty;

        public int? IndexId { get; set; }

        public string Identity { get; set; } = string.Empty;
    }
}
