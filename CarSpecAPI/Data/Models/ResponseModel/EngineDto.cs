namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class EngineDto
    {
        public int EngineId { get; set; }
        public string EngineName { get; set; } = null!;
        public string FuelType { get; set; }
        public byte NumberOfCylinders { get; set; }
        public byte NumberOfValves { get; set; }
        public decimal Displacement { get; set; }
        public decimal MaxPower { get; set; }
        public decimal MaxTorque { get; set; }
        public bool IsTurbocharged { get; set; }
        public string EmissionStandard { get; set; } = null!;
    }
}
