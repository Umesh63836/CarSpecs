using CarSpecAPI.Data.Models.ServiceModel;

namespace CarSpecAPI.Services
{
    public interface IRegistrationCalculator
    {
        RegistrationParameters Calculate(int stateCode, VehicleInfo vehicle);
    }
}