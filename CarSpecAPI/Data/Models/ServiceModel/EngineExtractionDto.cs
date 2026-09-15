using System.Text.Json.Serialization;

namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class EngineExtractionDto
    {
        [JsonPropertyName("engineRef")]
        public string EngineRef { get; set; } = "";

        [JsonPropertyName("engineName")]
        public string EngineName { get; set; } = "";

        [JsonPropertyName("fuelType")]
        public string? FuelType { get; set; }

        [JsonPropertyName("numberOfCylinders")]
        public int? NumberOfCylinders { get; set; }

        [JsonPropertyName("numberOfValves")]
        public int? NumberOfValves { get; set; }

        [JsonPropertyName("displacement")]
        public decimal? Displacement { get; set; }

        [JsonPropertyName("maxPower")]
        public decimal? MaxPower { get; set; }

        [JsonPropertyName("maxPowerRPM")]
        public decimal? MaxPowerRPM { get; set; }

        [JsonPropertyName("maxTorque")]
        public decimal? MaxTorque { get; set; }

        [JsonPropertyName("maxTorqueRPM")]
        public decimal? MaxTorqueRPM { get; set; }

        [JsonPropertyName("isTurbocharged")]
        public bool? IsTurbocharged { get; set; }

        [JsonPropertyName("emissionStandard")]
        public string? EmissionStandard { get; set; }

        [JsonPropertyName("aspiration")]
        public string? Aspiration { get; set; }

        [JsonPropertyName("engineType")]
        public string? EngineType { get; set; }

        [JsonPropertyName("batteryCapacityKWh")]
        public decimal? BatteryCapacityKWh { get; set; }

        [JsonPropertyName("motorPower")]
        public decimal? MotorPower { get; set; }

        [JsonPropertyName("motorTorque")]
        public decimal? MotorTorque { get; set; }
    }
}
