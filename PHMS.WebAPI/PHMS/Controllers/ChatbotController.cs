
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

            var response = await _chatbotAssistant.GetResponse(userInput.Input);
            return Ok(new { Response = response });
        }
    }

    public class UserInputModel
    {
        public string Input { get; set; }
    }
}
