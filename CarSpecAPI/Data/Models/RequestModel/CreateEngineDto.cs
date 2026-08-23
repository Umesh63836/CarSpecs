namespace CarSpecAPI.Data.Models.RequestModel
{
    using System.ComponentModel.DataAnnotations;

    public class CreateEngineDto
    {
        [Required]
        public string EngineName { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int FuelTypeId { get; set; }

        [Range(1, 20)]
        public byte NumberOfCylinders { get; set; }

        [Range(1, 32)]
        public byte NumberOfValves { get; set; }

        [Range(1, 10000)]
        public decimal Displacement { get; set; }

        [Range(0, 5000)]
        public decimal MaxPower { get; set; }

        [Range(0, 10000)]
        public decimal MaxTorque { get; set; }

        public bool IsTurbocharged { get; set; } = false;

        [Required]
        public string EmissionStandard { get; set; } = null!;
    }
}
