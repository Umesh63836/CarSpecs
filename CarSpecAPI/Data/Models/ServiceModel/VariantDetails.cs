using CarSpecAPI.Entities;

namespace CarSpecAPI.Data.Models.ServiceModel
{
    public class VariantDetails
    {
        public string? FuelType { get; set; } = string.Empty;

        public decimal? CubicCapacity { get; set; }

        public decimal? ExShowroomPrice { get; set; }

        public int? SeatingCapacity { get; set; }

        public decimal? KerbWeight { get; set; }

        public string? BodyType { get; set; } = string.Empty;

        public string? VehicleCategory { get; set; } = string.Empty;
    }
}
