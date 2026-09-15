using System.Text.Json.Serialization;

namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class VariantExtractionDto
    {
        [JsonPropertyName("variantName")]
        public string VariantName { get; set; } = "";

        [JsonPropertyName("engineRef")]
        public string? EngineRef { get; set; }

        [JsonPropertyName("transmissionRef")]
        public string? TransmissionRef { get; set; }

        [JsonPropertyName("drivetrainRef")]
        public string? DrivetrainRef { get; set; }

        [JsonPropertyName("exShowroomPrice")]
        public decimal? ExShowroomPrice { get; set; }

        [JsonPropertyName("kerbWeight")]
        public int? KerbWeight { get; set; }

        [JsonPropertyName("seatingCapacity")]
        public int? SeatingCapacity { get; set; }

        [JsonPropertyName("features")]
        public List<FeatureExtractionDto> Features { get; set; } = [];

        [JsonPropertyName("specifications")]
        public List<SpecificationExtractionDto> Specifications { get; set; } = [];
    }
}
