using CarSpecAPI.Data.Models.DataExtractionModel;
using CarSpecAPI.Services.AdministrationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Controllers
{
    [Authorize]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class StagedDataController : ControllerBase
    {
        private readonly IStagedDataService _service;
        private readonly ILogger<StagedDataController> _logger;

        public StagedDataController(
            IStagedDataService service,
            ILogger<StagedDataController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ============================================================
        // GET MODELS
        // ============================================================

        [HttpGet("models")]
        public async Task<ActionResult<PaginatedStagedModelResponseDto>> GetModels(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result =
                    await _service.GetModelsAsync(
                        pageNumber,
                        pageSize,
                        search,
                        cancellationToken);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve staged models.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while retrieving staged models."
                    });
            }
        }

        // ============================================================
        // GET COMPLETE MODEL
        // ============================================================

        [HttpGet("models/{importModelId:int}")]
        public async Task<ActionResult<StagedModelDetailDto>> GetModelByIdAsync(
            int importModelId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result =
                    await _service.GetModelAsync(
                        importModelId,
                        cancellationToken);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve staged model {ImportModelId}.",
                    importModelId);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while retrieving staged model data."
                    });
            }
        }

        // ============================================================
        // UPDATE MODEL
        // ============================================================

        [HttpPut("models/{importModelId:int}")]
        public async Task<IActionResult> UpdateModel(
            int importModelId,
            [FromBody] UpdateStagedModelDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateModelAsync(
                    importModelId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged model {ImportModelId}.",
                    importModelId);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while updating the staged model."
                    });
            }
        }

        // ============================================================
        // UPDATE VARIANT
        // ============================================================

        [HttpPut("variants/{importVariantId:int}")]
        public async Task<IActionResult> UpdateVariant(
            int importVariantId,
            [FromBody] UpdateStagedVariantDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateVariantAsync(
                    importVariantId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged variant {ImportVariantId}.",
                    importVariantId);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while updating the staged variant."
                    });
            }
        }

        // ============================================================
        // UPDATE POWERTRAIN
        // ============================================================

        [HttpPut("powertrains/{importPowertrainId:int}")]
        public async Task<IActionResult> UpdatePowertrain(
            int importPowertrainId,
            [FromBody] UpdateStagedPowertrainDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdatePowertrainAsync(
                    importPowertrainId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged powertrain {ImportPowertrainId}.",
                    importPowertrainId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while updating the staged powertrain."
                });
            }
        }

        // ============================================================
        // UPDATE TRANSMISSION
        // ============================================================

        [HttpPut("transmissions/{importTransmissionId:int}")]
        public async Task<IActionResult> UpdateTransmission(
            int importTransmissionId,
            [FromBody] UpdateStagedTransmissionDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateTransmissionAsync(
                    importTransmissionId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged transmission {ImportTransmissionId}.",
                    importTransmissionId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while updating the staged transmission."
                });
            }
        }

        // ============================================================
        // UPDATE DRIVETRAIN
        // ============================================================

        [HttpPut("drivetrains/{importDrivetrainId:int}")]
        public async Task<IActionResult> UpdateDrivetrain(
            int importDrivetrainId,
            [FromBody] UpdateStagedDrivetrainDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateDrivetrainAsync(
                    importDrivetrainId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged drivetrain {ImportDrivetrainId}.",
                    importDrivetrainId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while updating the staged drivetrain."
                });
            }
        }

        // ============================================================
        // UPDATE ENGINE
        // ============================================================

        [HttpPut("engines/{importEngineId:int}")]
        public async Task<IActionResult> UpdateEngine(
            int importEngineId,
            [FromBody] UpdateStagedEngineDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateEngineAsync(
                    importEngineId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged engine {ImportEngineId}.",
                    importEngineId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while updating the staged engine."
                });
            }
        }

        // ============================================================
        // UPDATE ENGINE PERFORMANCE
        // ============================================================

        [HttpPut("engine-performances/{importEnginePerformanceId:int}")]
        public async Task<IActionResult> UpdateEnginePerformance(
            int importEnginePerformanceId,
            [FromBody] UpdateStagedEnginePerformanceDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateEnginePerformanceAsync(
                    importEnginePerformanceId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged engine performance {Id}.",
                    importEnginePerformanceId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while updating staged engine performance."
                });
            }
        }

        // ============================================================
        // UPDATE MOTOR PERFORMANCE
        // ============================================================

        [HttpPut("motor-performances/{importMotorPerformanceId:int}")]
        public async Task<IActionResult> UpdateMotorPerformance(
            int importMotorPerformanceId,
            [FromBody] UpdateStagedMotorPerformanceDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateMotorPerformanceAsync(
                    importMotorPerformanceId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged motor performance {Id}.",
                    importMotorPerformanceId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while updating staged motor performance."
                });
            }
        }

        // ============================================================
        // UPDATE FEATURE VARIANT
        // ============================================================

        [HttpPut("feature-variants/{importFeatureVariantId:int}")]
        public async Task<IActionResult> UpdateFeatureVariant(
            int importFeatureVariantId,
            [FromBody] UpdateStagedFeatureDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateFeatureVariantAsync(
                    importFeatureVariantId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged feature variant {Id}.",
                    importFeatureVariantId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while updating staged feature."
                });
            }
        }

        // ============================================================
        // UPDATE SPECIFICATION VARIANT
        // ============================================================

        [HttpPut("specification-variants/{importSpecificationVariantId:int}")]
        public async Task<IActionResult> UpdateSpecificationVariant(
            int importSpecificationVariantId,
            [FromBody] UpdateStagedSpecificationDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateSpecificationVariantAsync(
                    importSpecificationVariantId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged specification variant {Id}.",
                    importSpecificationVariantId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while updating staged specification."
                });
            }
        }

        // ============================================================
        // UPDATE FUEL EFFICIENCY
        // ============================================================

        [HttpPut("fuel-efficiencies/{importFuelEfficiencyId:int}")]
        public async Task<IActionResult> UpdateFuelEfficiency(
            int importFuelEfficiencyId,
            [FromBody] UpdateStagedFuelEfficiencyDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateFuelEfficiencyAsync(
                    importFuelEfficiencyId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged fuel efficiency {Id}.",
                    importFuelEfficiencyId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while updating staged fuel efficiency."
                });
            }
        }

        // ============================================================
        // UPDATE WARRANTY
        // ============================================================

        [HttpPut("warranties/{importWarrantyId:int}")]
        public async Task<IActionResult> UpdateWarranty(
            int importWarrantyId,
            [FromBody] UpdateStagedWarrantyDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.UpdateWarrantyAsync(
                    importWarrantyId,
                    dto,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update staged warranty {Id}.",
                    importWarrantyId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while updating staged warranty."
                });
            }
        }

        // ============================================================
        // CREATE POWERTRAIN
        // ============================================================

        [HttpPost("powertrains")]
        public async Task<ActionResult<object>> CreatePowertrain(
            [FromBody] CreateStagedPowertrainDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreatePowertrainAsync(
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importPowertrainId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create staged powertrain.");

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating the staged powertrain."
                });
            }
        }

        [HttpPost("powertrains/{importPowertrainId:int}/engines")]
        public async Task<ActionResult<object>> CreateEngine(
    int importPowertrainId,
    [FromBody] CreateStagedEngineDto dto,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateEngineAsync(
                        importPowertrainId,
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importEngineId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create staged engine for powertrain {ImportPowertrainId}.",
                    importPowertrainId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating the staged engine."
                });
            }
        }



        [HttpPost("engines/{importEngineId:int}/performances")]
        public async Task<ActionResult<object>> CreateEnginePerformance(
    int importEngineId,
    [FromBody] CreateStagedEnginePerformanceDto dto,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateEnginePerformanceAsync(
                        importEngineId,
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importEnginePerformanceId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create staged engine performance.");

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating staged engine performance."
                });
            }
        }


        [HttpPost("powertrains/{importPowertrainId:int}/motor-performances")]
        public async Task<ActionResult<object>> CreateMotorPerformance(
    int importPowertrainId,
    [FromBody] CreateStagedMotorPerformanceDto dto,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateMotorPerformanceAsync(
                        importPowertrainId,
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importMotorPerformanceId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create staged motor performance.");

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating staged motor performance."
                });
            }
        }

        // ============================================================
        // CREATE TRANSMISSION
        // ============================================================

        [HttpPost("transmissions")]
        public async Task<ActionResult<object>> CreateTransmission(
            [FromBody] CreateStagedTransmissionDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateTransmissionAsync(
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importTransmissionId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create staged transmission.");

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating the staged transmission."
                });
            }
        }

        // ============================================================
        // CREATE DRIVETRAIN
        // ============================================================

        [HttpPost("drivetrains")]
        public async Task<ActionResult<object>> CreateDrivetrain(
            [FromBody] CreateStagedDrivetrainDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateDrivetrainAsync(
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importDrivetrainId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create staged drivetrain.");

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating the staged drivetrain."
                });
            }
        }

        // ============================================================
        // CREATE FEATURE
        // ============================================================

        [HttpPost("features")]
        public async Task<ActionResult<object>> CreateFeature(
            [FromBody] CreateStagedFeatureDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateFeatureAsync(
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importFeatureId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create staged feature.");

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating the staged feature."
                });
            }
        }

        // ============================================================
        // CREATE SPECIFICATION
        // ============================================================

        [HttpPost("specifications")]
        public async Task<ActionResult<object>> CreateSpecification(
            [FromBody] CreateStagedSpecificationDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateSpecificationAsync(
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importSpecificationId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create staged specification.");

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating the staged specification."
                });
            }
        }

        // ============================================================
        // CREATE WARRANTY
        // ============================================================

        [HttpPost("warranties")]
        public async Task<ActionResult<object>> CreateWarranty(
            [FromBody] CreateStagedWarrantyDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateWarrantyAsync(
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importWarrantyId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create staged warranty.");

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating the staged warranty."
                });
            }
        }

        // ============================================================
        // CREATE MODEL DIMENSIONS
        // ============================================================

        [HttpPost("models/{importModelId:int}/dimensions")]
        public async Task<ActionResult<object>> CreateModelDimensions(
            int importModelId,
            [FromBody] CreateStagedModelDimensionsDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateModelDimensionsAsync(
                        importModelId,
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importModelDimensionsId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create model dimensions for {ImportModelId}.",
                    importModelId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating model dimensions."
                });
            }
        }

        // ============================================================
        // CREATE FUEL EFFICIENCY
        // ============================================================

        [HttpPost("fuel-efficiencies")]
        public async Task<ActionResult<object>> CreateFuelEfficiency(
            [FromBody] CreateStagedFuelEfficiencyDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateFuelEfficiencyAsync(
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    importFuelEfficiencyId = id
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create staged fuel efficiency.");

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating staged fuel efficiency."
                });
            }
        }

        // ============================================================
        // CREATE / GET FUEL TYPE
        // ============================================================

        [HttpPost("fuel-types")]
        public async Task<ActionResult<object>> CreateFuelType(
            [FromBody] CreateStagedFuelTypeDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var id =
                    await _service.CreateFuelTypeAsync(
                        dto,
                        cancellationToken);

                return Ok(new
                {
                    fuelTypeId = id,
                    fuelTypeName = dto.FuelTypeName?.Trim()
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create fuel type.");

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while creating the fuel type."
                });
            }
        }

        // ============================================================
        // ATTACH FUEL EFFICIENCY TO ANOTHER VARIANT
        // ============================================================


        [HttpPost(
            "fuel-efficiencies/{importFuelEfficiencyId:int}/variants/{importVariantId:int}")]
        public async Task<IActionResult> AttachFuelEfficiencyToVariant(
            int importFuelEfficiencyId,
            int importVariantId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.AttachFuelEfficiencyToVariantAsync(
                    importFuelEfficiencyId,
                    importVariantId,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to attach fuel efficiency {FuelEfficiencyId} to variant {VariantId}.",
                    importFuelEfficiencyId,
                    importVariantId);

                return StatusCode(500, new
                {
                    message =
                        "An unexpected error occurred while attaching fuel efficiency."
                });
            }
        }


        [HttpGet("powertrain-dependencies")]
        public async Task<ActionResult<StagedPowertrainDependenciesDto>> GetPowertrainDependencies(
        CancellationToken cancellationToken = default)
        {
            var result =
                await _service.GetPowertrainDependenciesAsync(
                    cancellationToken);

            return Ok(result);
        }

        [HttpGet("feature-specification-catalog")]
        public async Task<ActionResult<StagedFeatureSpecificationCatalogDto>> GetFeatureSpecificationCatalogAsync(CancellationToken cancellationToken = default)
        {
            var result = await _service.GetFeatureSpecificationCatalogAsync(cancellationToken);

            return Ok(result);
        }

        // ============================================================
        // GET POWERTRAINS FOR MODEL
        // ============================================================

        [HttpGet("models/{importModelId:int}/powertrains")]
        public async Task<
            ActionResult<List<StagedPowertrainLookupDto>>>
            GetPowertrainsForModel(
                int importModelId)
        {
            try
            {
                var result =
                    await _service
                        .GetPowertrainsForModelAsync(
                            importModelId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        // ============================================================
        // GET TRANSMISSIONS FOR MODEL
        // ============================================================

        [HttpGet("models/{importModelId:int}/transmissions")]
        public async Task<
            ActionResult<List<StagedTransmissionLookupDto>>>
            GetTransmissionsForModel(
                int importModelId)
        {
            try
            {
                var result =
                    await _service
                        .GetTransmissionsForModelAsync(
                            importModelId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        // ============================================================
        // GET DRIVETRAINS FOR MODEL
        // ============================================================

        [HttpGet("models/{importModelId:int}/drivetrains")]
        public async Task<
            ActionResult<List<StagedDrivetrainLookupDto>>>
            GetDrivetrainsForModel(
                int importModelId)
        {
            try
            {
                var result =
                    await _service
                        .GetDrivetrainsForModelAsync(
                            importModelId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        // ============================================================
        // REPLACE POWERTRAIN REFERENCE
        // ============================================================

        [HttpPut(
            "variants/{importVariantId:int}/powertrain-reference")]
        public async Task<
            ActionResult<ReplaceStagedPowertrainReferenceResultDto>>
            ReplaceVariantPowertrainReference(
                int importVariantId,
                [FromBody]
            ReplaceStagedPowertrainReferenceDto dto)
        {
            try
            {
                var result =
                    await _service
                        .ReplaceVariantPowertrainReferenceAsync(
                            importVariantId,
                            dto);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // ============================================================
        // REPLACE TRANSMISSION REFERENCE
        // ============================================================

        [HttpPut(
            "variants/{importVariantId:int}/transmission-reference")]
        public async Task<
            ActionResult<ReplaceStagedTransmissionReferenceResultDto>>
            ReplaceVariantTransmissionReference(
                int importVariantId,
                [FromBody]
            ReplaceStagedTransmissionReferenceDto dto)
        {
            try
            {
                var result =
                    await _service
                        .ReplaceVariantTransmissionReferenceAsync(
                            importVariantId,
                            dto);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // ============================================================
        // REPLACE DRIVETRAIN REFERENCE
        // ============================================================

        [HttpPut(
            "variants/{importVariantId:int}/drivetrain-reference")]
        public async Task<
            ActionResult<ReplaceStagedDrivetrainReferenceResultDto>>
            ReplaceVariantDrivetrainReference(
                int importVariantId,
                [FromBody]
            ReplaceStagedDrivetrainReferenceDto dto)
        {
            try
            {
                var result =
                    await _service
                        .ReplaceVariantDrivetrainReferenceAsync(
                            importVariantId,
                            dto);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}