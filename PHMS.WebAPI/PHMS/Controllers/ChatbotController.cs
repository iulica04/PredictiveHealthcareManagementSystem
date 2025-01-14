using Application.AI;
using Microsoft.AspNetCore.Mvc;

namespace PHMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly ChatbotAssistant _chatbotAssistant;

        public ChatbotController(ChatbotAssistant chatbotAssistant)
        {
            _chatbotAssistant = chatbotAssistant;
        }

        [HttpPost("get-response")]
        public async Task<IActionResult> GetResponse([FromBody] UserInputModel userInput)
        {
            if (userInput == null || string.IsNullOrEmpty(userInput.Input))
            {
                return BadRequest("Invalid input.");
            }

            try
            {
                var response = await _chatbotAssistant.GetResponse(userInput.Input);
                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class UserInputModel
    {
        public required string Input { get; set; }
    }
}
