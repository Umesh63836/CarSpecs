namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class VariantSpecsDto
    {
        public int EngineId { get; set; }

        public string Engine { get; set; } = string.Empty;

        public byte? NoOfCyl { get; set; }

        public byte? NoOfValves { get; set; }

        public decimal? Displacement { get; set; }

        public decimal? MaxPower { get; set; }

        public decimal? MaxTorque { get; set; }

        public bool isTurbocharged { get; set; } = false;

        public string EmmissionStandard { get; set; } = string.Empty;

        public string TransmissionType { get; set; } = string.Empty;

        public string FuelType {  get; set; } = string.Empty;

        public byte? NoOfGears { get; set; }

        public string Drivetrain { get; set;} = string.Empty;

        public string VarientImageURL { get; set; } = string.Empty;

    }
}
