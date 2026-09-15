using System.Text.Json.Serialization;

namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class DrivetrainExtractionDto
    {
        [JsonPropertyName("drivetrainRef")]
        public string DrivetrainRef { get; set; } = "";

        [JsonPropertyName("drivetrainType")]
        public string DrivetrainType { get; set; } = "";

        [JsonPropertyName("differentialType")]
        public string? DifferentialType { get; set; }
    }
}
