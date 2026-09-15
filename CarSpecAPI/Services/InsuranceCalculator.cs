using CarSpecAPI.Data.Models.ServiceModel;

namespace CarSpecAPI.Services
{
    public class InsuranceCalculator : IInsuranceCalculator
    {
        public decimal? Calculate(decimal exShowroomPrice, string? vehicleCategory)
        {
            decimal baseRate;

            if (exShowroomPrice < 1000000)
            {
                baseRate = 0.0425m;
            }
            else if (exShowroomPrice <= 2000000)
            {
                baseRate = 0.0440m;
            }
            else if (exShowroomPrice <= 3000000)
            {
                baseRate = 0.0425m;
            }
            else
            {
                baseRate = 0.0385m;
            }

            decimal adjustment = vehicleCategory.ToLowerInvariant() switch
            {
                "hatchback" => -0.0010m,
                "sedan" => 0m,
                "compact suv" => 0.0010m,
                "mid-size suv" => 0.0020m,
                "large suv" => 0.0030m,
                "suv" => 0.0030m,
                "luxury" => 0.0050m,
                _ => 0m
            };

            decimal effectiveRate = baseRate + adjustment;

            decimal amount = exShowroomPrice * effectiveRate;

            return Math.Round(amount);
        }
    }
}
