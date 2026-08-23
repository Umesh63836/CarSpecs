using CarSpecAPI.Data;
using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CarSpecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandsService brandsService;

        public BrandsController(IBrandsService brandsService)
        {
            this.brandsService = brandsService;
        }

        [HttpGet]
        [EnableRateLimiting("api")]
        public async Task<IActionResult> GetBrands()
        {
            var brands =await brandsService.GetAllBrandsAsync();
            return Ok(brands);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBrand(CreateBrandDto dto)
        {
            var response = await this.brandsService.CreateBrandAsync(dto);

            return Ok(response);
        }
    }
}
