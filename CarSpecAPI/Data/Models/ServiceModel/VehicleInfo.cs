namespace CarSpecAPI.Data.Models.ServiceModel
{
    public class VehicleInfo
    {
        public decimal ExShowroomPrice { get; set; }

        public string FuelType { get; set; } = string.Empty;

        public decimal? EngineCc { get; set; }

        public decimal? KerbWeight { get; set; }

        public string? BodyType { get; set; } = string.Empty;

        public int? SeatingCapacity { get; set; }

        public string? VehicleCategory { get; set; } = string.Empty;

        public bool IsElectric => FuelType.Equals("Electric", StringComparison.OrdinalIgnoreCase);

        public bool IsDiesel => FuelType.Equals("Diesel", StringComparison.OrdinalIgnoreCase);

        public bool IsPetrol => FuelType.Equals("Petrol", StringComparison.OrdinalIgnoreCase);

        public bool IsCng =>  FuelType.Equals("CNG", StringComparison.OrdinalIgnoreCase);
    }
}
