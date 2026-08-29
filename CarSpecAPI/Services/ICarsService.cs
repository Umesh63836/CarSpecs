using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;

namespace CarSpecAPI.Services
{
    public interface ICarsService
    {
        Task<CarSearchResponseAiDto> SearchCarsAiAsync(CarsSearchRequest request);
        Task<CarSearchResponseDto> SearchCarsAsync(CarsSearchRequest request);
    }
}