namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class VariantDto
    {
        public int VariantId { get; set; }

        public string VariantName { get; set; } = string.Empty;

        public string FuelType { get; set; } = string.Empty;

        public decimal? CubicCapacity { get; set; }

        public bool isTurbocharged { get; set; } = false;

        public string TransmissionType {  get; set; } = string.Empty;

        public decimal? MaxPower { get; set; }

        public decimal? MaxTorque { get; set; }

        public decimal? ExShowroomPrice { get; set; }
    }
}
