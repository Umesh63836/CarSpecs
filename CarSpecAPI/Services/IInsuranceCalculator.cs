namespace CarSpecAPI.Services
{
    public interface IInsuranceCalculator
    {
        decimal? Calculate(decimal exShowroomPrice, string? vehicleCategory);
    }
}