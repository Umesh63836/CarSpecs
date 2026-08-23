using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CarSpecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecificationsController : ControllerBase
    {
        private readonly ISpecificationService specificationService;

        public SpecificationsController(ISpecificationService specificationService)
        {
            this.specificationService = specificationService;
        }

        [HttpGet("variant/{id}")]
        [EnableRateLimiting("api")]
        public async Task<IActionResult> GetSpecsByVariant(int id)
        {
            var result = await specificationService.GetVariantSpecsAsync(id);

            return result == null ? NotFound() : Ok(result);
        }

        [Authorize]
        [HttpGet("allengines")]
        public async Task<IActionResult> GetAllEngines()
        {
            var response = await this.specificationService.GetAllEnginesAsync();

            return Ok(response);
        }

        [Authorize]
        [HttpGet("alldrivetrains")]
        public async Task<IActionResult> GetAllDrivetrains()
        {
            var response = await this.specificationService.GetAllDrivetrainsAsync();

            return Ok(response);
        }

        [Authorize]
        [HttpGet("alltransmissions")]
        public async Task<IActionResult> GetAllTransmissions()
        {
            var response = await this.specificationService.GetAllTransmissionsAsync();

            return Ok(response);
        }

        [Authorize]
        [HttpGet("allfueltypes")]
        public async Task<IActionResult> GetAllFuelTypes()
        {
            var response = await this.specificationService.GetAllFuelTypesAsync();

            return Ok(response);
        }

        [Authorize]
        [HttpPost("engine")]
        public async Task<IActionResult> CreateNewEngine([FromBody] CreateEngineDto dto)
        {
            var response = await this.specificationService.CreateEngineAsync(dto);

            return response == null ? NotFound("Fueltype doesnot exist") : Ok(response);
        }

        [Authorize]
        [HttpPost("drivetrain")]
        public async Task<IActionResult> CreateNewDrivetrain([FromBody] CreateDrivetrainDto dto)
        {
            var response = await this.specificationService.CreateDrivetrainAsync(dto);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("transmission")]
        public async Task<IActionResult> CreateNewTransmission([FromBody] CreateTransmissionDto dto)
        {
            var response = await this.specificationService.CreateTransmissionAsync(dto);

            return Ok(response);
        }
    }
}
