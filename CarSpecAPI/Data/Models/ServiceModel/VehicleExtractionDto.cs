using System.Text.Json.Serialization;

namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class VehicleExtractionDto
    {
        [JsonPropertyName("brandName")]
        public string BrandName { get; set; } = "";

        [JsonPropertyName("modelName")]
        public string ModelName { get; set; } = "";

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("bodyType")]
        public string? BodyType { get; set; }
    }
}
