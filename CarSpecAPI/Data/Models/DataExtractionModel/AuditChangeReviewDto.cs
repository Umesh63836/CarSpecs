using System.Text.Json;

namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public class AuditChangeReviewDto
    {
        public string ChangeId { get; set; } = string.Empty;

        public string IssueType { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Severity { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string Collection { get; set; } = string.Empty;

        public int? IndexId { get; set; }

        public string Identity { get; set; } = string.Empty;

        public JsonElement? Data { get; set; }

        public int? PageNumber { get; set; }

        public string? Evidence { get; set; }

        public string? Reason { get; set; }

        public string Decision { get; set; } = "Pending";

        public string? ModifiedData { get; set; }

        public string? Notes { get; set; }
    }
}
