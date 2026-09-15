using CarSpecAPI.Data.Models.ResponseModel;

namespace CarSpecAPI.Services
{
    public interface ILocationService
    {
        Task<List<LocationSearchDto>> SearchLocationsAsync(string searchTerm, int limit = 10);
    }
}