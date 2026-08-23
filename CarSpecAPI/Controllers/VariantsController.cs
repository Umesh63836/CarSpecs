using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


namespace CarSpecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariantsController : ControllerBase
    {
        private readonly IVariantsService variantsService;

        public VariantsController(IVariantsService variantsService)
        {
            this.variantsService = variantsService;
        }

        [HttpGet("model/{id}")]
        [EnableRateLimiting("api")]
        public async Task<IActionResult> GetVariantsByModelId(int id)
        {
            var result = await variantsService.GetVariantsAsync(id);        

            return result.Count == 0 ? NotFound() : Ok(result);
        }

        [Authorize]
        [HttpPost("{modelId}/variants")]
        public async Task<IActionResult> CreateVariant(int modelId, CreateVariantDto dto)
        {
            var result = await variantsService.CreateVariantAsync(modelId, dto);

            return result == null ? NotFound("Model not found") : Ok(result);
        }
    }
}
