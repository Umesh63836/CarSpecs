using System.Text.Json;

namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public class AuditChangeDto
    {
        public string ChangeId { get; set; } = string.Empty;

        public string IssueType { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Severity { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public AuditChangeTargetDto Target { get; set; } = new();

        public JsonElement? Data { get; set; }

        public int? PageNumber { get; set; }

        public string? Evidence { get; set; }

        public string? Reason { get; set; }
    }
}
