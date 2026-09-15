using System.Text.Json.Serialization;

namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class TransmissionExtractionDto
    {
        [JsonPropertyName("transmissionRef")]
        public string TransmissionRef { get; set; } = "";

        [JsonPropertyName("transmissionType")]
        public string TransmissionType { get; set; } = "";

        [JsonPropertyName("numberOfGears")]
        public int? NumberOfGears { get; set; }

        [JsonPropertyName("hasManualOverride")]
        public bool? HasManualOverride { get; set; }

        [JsonPropertyName("hasPaddleShifters")]
        public bool? HasPaddleShifters { get; set; }
    }
}
