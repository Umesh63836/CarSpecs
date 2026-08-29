namespace CarSpecAPI.Data.Models.RequestModel
{
    public class CarsSearchRequest
    {
        public string? Brand { get; set; } 
        public string? Model { get; set; } 
        public decimal? Displacement { get; set; } 
        public decimal? MaxPower { get; set; } 
        public decimal? MinPower { get; set; } 
        public decimal? MaxTorque { get; set; } 
        public decimal? MinTorque { get; set; } 
        public bool? IsTurbocharged { get; set; } 
        public string? EmissionStandard { get; set; } 
        public string? TransmissionType { get; set; } 
        public byte? NumberOfGears { get; set; } 
        public string? DrivetrainType { get; set; } 
        public string? FuelType { get; set; } 
        public decimal? MaxPrice { get; set; } 
        public decimal? MinPrice { get; set; }
    }
}
