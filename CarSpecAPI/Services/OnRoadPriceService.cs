using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Data.Models.ServiceModel;

namespace CarSpecAPI.Services
{
    public class OnRoadPriceService : IOnRoadPriceService
    {
        private readonly IVariantsService variantsService;
        private readonly IRegistrationCalculator registrationCalculator;
        private readonly IInsuranceCalculator insuranceCalculator;
        private readonly ITcsCalculator tcsCalculator;
        public const decimal Fastag = 500m;

        public OnRoadPriceService(IVariantsService variantsService, IRegistrationCalculator registrationCalculator, IInsuranceCalculator insuranceCalculator, ITcsCalculator tcsCalculator)
        {

            this.variantsService = variantsService;
            this.registrationCalculator = registrationCalculator;
            this.insuranceCalculator = insuranceCalculator;
            this.tcsCalculator = tcsCalculator;
        }

        public async Task<OnRoadPriceDto> CalculateAsync(int variantId, int stateId)
        {
            var variant = await variantsService.GetVariantDetailsAsync(variantId);

            if (variant == null)
                throw new KeyNotFoundException($"Variant {variantId} not found.");

            VehicleInfo vehicle = new VehicleInfo
            {
                ExShowroomPrice = Convert.ToDecimal(variant.ExShowroomPrice),
                FuelType = variant.FuelType,
                EngineCc = variant.CubicCapacity,
                KerbWeight = variant.KerbWeight,
                BodyType = variant.BodyType,
                VehicleCategory = variant.VehicleCategory
            };

            var registration = registrationCalculator.Calculate(stateId, vehicle);

            decimal? insuranceAmount = insuranceCalculator.Calculate(Convert.ToDecimal(variant.ExShowroomPrice), variant.VehicleCategory);

            decimal tcs = tcsCalculator.Calculate(Convert.ToDecimal(variant.ExShowroomPrice));

            decimal total = (Convert.ToDecimal(variant.ExShowroomPrice) + Convert.ToDecimal(registration.TotalRegistrationCharges) + Convert.ToDecimal(insuranceAmount) + tcs + Fastag);

            return new OnRoadPriceDto
            {
                VariantId = variantId,
                StateId = stateId,
                ExShowroomPrice = Convert.ToDecimal(variant.ExShowroomPrice),
                Registration = registration,
                Insurance = insuranceAmount,
                Tcs = tcs,
                Fastag = Fastag,
                TotalOnRoadPrice = total
            };
        }
    }
}
