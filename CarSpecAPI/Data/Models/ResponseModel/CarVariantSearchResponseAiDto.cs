namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class CarVariantSearchResponseAiDto
    {
        public int VariantId { get; set; }

        public string VariantName { get; set; } = string.Empty;

        public decimal? ExShowroomPrice { get; set; }

        public decimal? Displacement { get; set; }

        public decimal? MaxPower { get; set; }

        public decimal? MaxTorque { get; set; }

        public bool? IsTurbocharged { get; set; }

        public string? EmissionStandard { get; set; } = string.Empty;

        public string? FuelType { get; set; } = string.Empty;

        public string? TransmissionType { get; set; } = string.Empty;

        public byte? NumberOfGears { get; set; }

        public string? DrivetrainType { get; set; } = string.Empty;
    }
}
