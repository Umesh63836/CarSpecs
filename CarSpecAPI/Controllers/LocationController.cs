using CarSpecAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarSpecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService locationService;

        public LocationController(ILocationService locationService)
        {
            this.locationService = locationService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string location, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest(new{ message = "Search term is required." });
            }

            var results = await locationService
                .SearchLocationsAsync(location, limit);

            return Ok(results);
        }
    }
}
