using System.Text.Json.Serialization;

namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class SpecificationExtractionDto
    {
        [JsonPropertyName("specificationId")]
        public int SpecificationId { get; set; }

        [JsonPropertyName("numericValue")]
        public decimal? NumericValue { get; set; }

        [JsonPropertyName("textValue")]
        public string? TextValue { get; set; }

        [JsonPropertyName("booleanValue")]
        public bool? BooleanValue { get; set; }

        [JsonPropertyName("unit")]
        public string? Unit { get; set; }

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
