using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarSpecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnRoadPriceController : ControllerBase
    {
        private readonly IOnRoadPriceService service;

        public OnRoadPriceController(IOnRoadPriceService service)
        {
            this.service = service;
        }

        [HttpGet("{variantId:int}")]
        public async Task<ActionResult<OnRoadPriceDto>> Get(int variantId, [FromQuery] int stateId)
        {
            try
            {
                var result = await service.CalculateAsync(variantId, stateId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new{ message = ex.Message });
            }
        }
    }
}
