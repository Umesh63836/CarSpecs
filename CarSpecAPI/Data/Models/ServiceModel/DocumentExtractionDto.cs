using System.Text.Json.Serialization;

namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class DocumentExtractionDto
    {
        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; } = "";

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = "";
    }
}
