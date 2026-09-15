using CarspecAPI.Services;
using CarSpecAPI.Data.Models.DataExtractionModel;
using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Services;
using CarsSpecAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CarsSpecAPI.Services.ImportBatchService;

namespace CarSpecAPI.Controllers
{
    [Authorize]
    [Route("api/import")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IBrochureImportService importService;
        private readonly IBrochurePageProcessingService brochurePageProcessingService;
        private readonly IImportBatchService importBatchService;

        public ImportController(IBrochureImportService importService, IBrochurePageProcessingService brochurePageProcessingService, IImportBatchService importBatchService)
        {
            this.importService = importService;
            this.brochurePageProcessingService = brochurePageProcessingService;
            this.importBatchService = importBatchService;
        }

        [Authorize]
        [HttpPost("brochure")]
        [RequestSizeLimit(25 * 1024 * 1024)]
        public async Task<IActionResult> UploadBrochure([FromForm] BrochureUploadRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await importService.UploadAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new
                    {
                        message = "Failed to upload brochure."
                    });
            }
        }

        [HttpPost("brochurePages")]
        public async Task<IActionResult> SelectPages([FromBody] SelectPagesRequestDto request, CancellationToken cancellationToken)
        {
            await brochurePageProcessingService.ProcessSelectedPagesAsync(request.ImportDocumentId, request.FileName, request.SelectedPages, cancellationToken);
            return Ok(new
            {
                success = true,
                importDocumentId = request.ImportDocumentId,
                selectedPages = request.SelectedPages
            });
        }

        [HttpGet("batches")]
        public async Task<ActionResult<PaginatedImportBatchResponseDto>> GetImportBatches([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1)
            {
                return BadRequest(new
                {
                    message = "Page number must be greater than or equal to 1."
                });
            }
            var result =
                await importBatchService.GetImportBatchesAsync(
                    pageNumber,
                    10,
                    cancellationToken
                );
            return Ok(result);
        }

        [HttpPost("import-batches/{importBatchId:int}/stage")]
        public async Task<ActionResult<StageBatchResultDto>> StageBatch(int importBatchId, CancellationToken cancellationToken)
        {
            try
            {
                var result =
                    await importBatchService.StageBatchAsync(
                        importBatchId,
                        cancellationToken);

                return Ok(result);
            }
            catch (ImportBatchValidationException ex)
            {
                return BadRequest(new
                {
                    importBatchId = ex.ImportBatchId,
                    message = "FinalJson validation failed.",
                    errors = ex.Errors
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    importBatchId,
                    message = ex.Message
                });
            }
        }
    }
}
