using CarSpecAPI.Data.Models.ResponseModel;

namespace CarSpecAPI.Services
{
    public interface IOnRoadPriceService
    {
        Task<OnRoadPriceDto> CalculateAsync(int variantId, int stateId);
    }
}