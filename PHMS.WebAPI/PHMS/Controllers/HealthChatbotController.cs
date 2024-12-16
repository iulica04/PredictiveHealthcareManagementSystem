using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.AIML;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HealthChatbotController : ControllerBase
    {
        private readonly DiseasePredictionService _diseasePredictionService;

        public HealthChatbotController(DiseasePredictionService diseasePredictionService)
        {
            _diseasePredictionService = diseasePredictionService;
        }

        [HttpPost("predict")]
        public IActionResult PredictSymptoms([FromBody] SymptomDTO symptoms)
        {
            if (string.IsNullOrWhiteSpace(symptoms.Symptoms))
                return BadRequest("Simptomele nu pot fi goale.");

            // Obține predicția bolii pe baza simptomelor
            var predictedDisease = _diseasePredictionService.PredictDisease(symptoms.Symptoms);

            return Ok(new { PredictedDisease = predictedDisease });
        }
    }

    // DTO pentru simptomele primite de la utilizator
    public class SymptomDTO
    {
        public string Symptoms { get; set; }
    }
}
