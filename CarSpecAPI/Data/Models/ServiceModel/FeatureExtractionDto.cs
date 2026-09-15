using System.Text.Json.Serialization;

namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class FeatureExtractionDto
    {
        [JsonPropertyName("featureId")]
        public int FeatureId { get; set; }

        [JsonPropertyName("available")]
        public bool? Available { get; set; }

        [JsonPropertyName("valueOptionIds")]
        public List<int> ValueOptionIds { get; set; } = [];

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("sourceColumn")]
        public string? SourceColumn { get; set; }

        [JsonPropertyName("pageNumber")]
        public int? PageNumber { get; set; }

        [JsonPropertyName("evidence")]
        public string? Evidence { get; set; }

        [JsonPropertyName("confidence")]
        public decimal? Confidence { get; set; }
    }
}
