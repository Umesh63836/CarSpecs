using System.Text.Json;

namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public class AdminAuditDecisionDto
    {
        public string ChangeId { get; set; } = string.Empty;

        public string Decision { get; set; } = "Pending";

        public JsonElement? ModifiedData { get; set; }

        public string? Notes { get; set; }
    }
}
