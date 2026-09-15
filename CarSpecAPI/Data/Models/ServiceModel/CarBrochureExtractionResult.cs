using System.Text.Json.Serialization;

namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class CarBrochureExtractionResult
    {
        [JsonPropertyName("document")]
        public DocumentExtractionDto Document { get; set; } = new();

        [JsonPropertyName("vehicle")]
        public VehicleExtractionDto Vehicle { get; set; } = new();

        [JsonPropertyName("engines")]
        public List<EngineExtractionDto> Engines { get; set; } = [];

        [JsonPropertyName("transmissions")]
        public List<TransmissionExtractionDto> Transmissions { get; set; } = [];

        [JsonPropertyName("drivetrains")]
        public List<DrivetrainExtractionDto> Drivetrains { get; set; } = [];

        [JsonPropertyName("variants")]
        public List<VariantExtractionDto> Variants { get; set; } = [];

        [JsonPropertyName("warnings")]
        public List<string> Warnings { get; set; } = [];
    }
}
