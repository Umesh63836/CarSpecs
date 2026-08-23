using CarSpecAPI.Data.Models.ResponseModel;

namespace CarSpecAPI.Services
{
    public interface ISearchService
    {
        public Task<List<SearchResultDto>> SearchAsync(string searchParameter);
    }
}
