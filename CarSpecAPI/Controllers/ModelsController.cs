using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly IModelsService modelsService;

        public ModelsController(IModelsService modelsService)
        {
            this.modelsService = modelsService;
        }

        [HttpGet("Brand/{id}")]
        [EnableRateLimiting("api")]
        public async Task<IActionResult> GetModelsByBrand(int id)
        {
            var result = await modelsService.GetModelsAsync(id);

            return result.Count == 0 ? NotFound(): Ok(result);
        }

        [HttpGet("Model/{id}")]
        public async Task<IActionResult> GetModelByIdAsync(int id)
        {
            var result = await modelsService.GetModelByIdAsync(id);

            return result ==null ? NotFound() : Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateModel(CreateModelDto dto)
        {
            var response = await this.modelsService.CreateModelAsync(dto);

            return response == null ? NotFound("Brand doesnot exist") : Ok(response);
        }
    }
}
