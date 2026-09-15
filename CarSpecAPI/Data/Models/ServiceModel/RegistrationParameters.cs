namespace CarSpecAPI.Data.Models.ServiceModel
{
    public class RegistrationParameters
    {
        public decimal RoadTax { get; set; }

        public decimal RoadTaxCess { get; set; }

        public decimal InfrastructureCess { get; set; }

        public decimal RegistrationFee { get; set; }

        public decimal HsrpCharge { get; set; }

        public decimal GreenTax { get; set; }

        public decimal OtherCharges { get; set; }

        public decimal TotalRegistrationCharges =>
            RoadTax
            + RoadTaxCess
            + InfrastructureCess
            + RegistrationFee
            + HsrpCharge
            + GreenTax
            + OtherCharges;
    }
}
