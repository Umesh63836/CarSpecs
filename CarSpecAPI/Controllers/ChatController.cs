using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarSpecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IAIService aIService;

        public ChatController(IAIService aIService)
        {
            this.aIService = aIService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
        {
            var response = await aIService.GetResponseAsync(request.Message);

            return Ok(new
            {
                message = response
            });
        }
    }
}
