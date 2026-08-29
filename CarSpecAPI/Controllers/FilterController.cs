using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarSpecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly ICarsService carsService;

        public FilterController(ICarsService carsService)
        {
            this.carsService = carsService;
        }

        [HttpGet()]
        public async Task<IActionResult> SearchCars([FromQuery] CarsSearchRequest request)
        {
            var result =
                await carsService.SearchCarsAsync(request);

            return Ok(result);
        }
    }
}
