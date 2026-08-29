namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class CarVariantSearchResponseDto
    {
        public int VariantId { get; set; }

        public string VariantName { get; set; } = string.Empty;

        public decimal? ExShowroomPrice { get; set; }

        public decimal? MaxPower { get; set; }

        public string? FuelType { get; set; } = string.Empty;

        public string? TransmissionType { get; set; } = string.Empty;

        public string? DrivetrainType { get; set; } = string.Empty;
    }
}
