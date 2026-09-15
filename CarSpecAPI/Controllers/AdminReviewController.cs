using CarspecAPI.Services;
using CarSpecAPI.Data.Models.DataExtractionModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CarspecAPI.Services.AdminReviewService;

namespace CarspecAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/admin-review")]
public class AdminReviewController : ControllerBase
{
    private readonly AdminReviewService _adminReviewService;

    public AdminReviewController(
        AdminReviewService adminReviewService)
    {
        _adminReviewService = adminReviewService;
    }


    // ============================================================
    // 1. GET REVIEW DATA
    // ============================================================
    //
    // GET:
    // /api/admin-review/{batchId}
    //
    // Returns:
    // - Model1 JSON
    // - AI audit changes
    // - Existing admin decisions
    // - Warnings
    //
    // ============================================================

    [HttpGet("{batchId:int}")]
    public async Task<ActionResult<BrochureImportReviewDto>> GetReview(
        int batchId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result =
                await _adminReviewService.GetReviewAsync(
                    batchId,
                    cancellationToken);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }


    // ============================================================
    // 2. SAVE ADMIN DECISIONS
    // ============================================================
    //
    // POST:
    // /api/admin-review/{batchId}/decisions
    //
    // Body:
    //
    // {
    //   "reviewedBy": 1,
    //   "decisions": [
    //     {
    //       "changeId": "C001",
    //       "decision": "Accepted",
    //       "modifiedData": null,
    //       "notes": "Verified"
    //     },
    //     {
    //       "changeId": "C002",
    //       "decision": "Modified",
    //       "modifiedData": "{\"featureId\":29,...}",
    //       "notes": "Corrected applicability"
    //     },
    //     {
    //       "changeId": "C003",
    //       "decision": "Rejected",
    //       "modifiedData": null,
    //       "notes": "Not supported by brochure"
    //     }
    //   ]
    // }
    //
    // ============================================================

    [HttpPost("{batchId:int}/decisions")]
    public async Task<IActionResult> SaveDecisions(
        int batchId,
        [FromBody] SaveAdminReviewRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Request body is required."
                });
            }

            if (request.Decisions == null ||
                request.Decisions.Count == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "At least one decision is required."
                });
            }

            await _adminReviewService.SaveReviewAsync(
                batchId,
                request.Decisions,
                request.ReviewedBy,
                cancellationToken);

            return Ok(new
            {
                success = true,
                message = "Admin review decisions saved successfully.",
                importBatchId = batchId
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }


    // ============================================================
    // 3. BUILD FINAL JSON
    // ============================================================
    //
    // POST:
    // /api/admin-review/{batchId}/build-final
    //
    // This:
    //
    // Model1Json
    //      +
    // ImportAuditChange decisions
    //      ↓
    // Final JSON
    //
    // ============================================================

    [HttpPost("{batchId:int}/build-final")]
    public async Task<IActionResult> BuildFinalJson(
        int batchId,
        CancellationToken cancellationToken)
    {
        try
        {
            var finalJson =
                await _adminReviewService.BuildFinalJsonAsync(
                    batchId,
                    cancellationToken);

            /*
             * Return JSON as JSON rather than as an escaped string.
             *
             * This is useful for the frontend because it receives:
             *
             * {
             *   "document": {...},
             *   "vehicle": {...},
             *   ...
             * }
             *
             * instead of:
             *
             * "{\r\n  \"document\": ..."
             */

            return Content(
                finalJson,
                "application/json");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

}
