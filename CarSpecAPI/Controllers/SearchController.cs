using CarSpecAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CarSpecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService searchService;

        public SearchController(ISearchService searchService)
        {
            this.searchService = searchService;
        }


        [HttpGet("search/{searchParameter}")]
        [EnableRateLimiting("search")]
        public async Task<IActionResult> Search(string searchParameter)
        {
            var results = await searchService.SearchAsync(searchParameter);

            return Ok(results);
        }
    }
}
