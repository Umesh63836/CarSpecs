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

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ConversationId)) 
            { 
                return BadRequest("ConversationId is required."); 
            } 

            if (string.IsNullOrWhiteSpace(request.Message)) 
            { 
                return BadRequest("Message is required."); 
            } 

            var response = await aIService.GetResponseAsync(request.ConversationId, request.Message ); 
            return Ok(new { message = response });
        }
    }
}
