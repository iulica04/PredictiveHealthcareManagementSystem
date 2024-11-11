using Application.Commands.MedicalConditionCommands;
using Application.DTOs;
using Application.Queries.MedialConditionQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MedicalConditionController : ControllerBase
    {
        private readonly IMediator mediator;

        public MedicalConditionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMedicalCondition(CreateMedicalConditionCommand command)
        {
            var id = await mediator.Send(command);

            return CreatedAtAction(
                nameof(GetMedicalConditionForPatient), // Reference the action by name
                new { patientId = command.PatientId, medicalConditionId = id }, // Pass the patientId and id as parameters
                new { id }); 
        }

        [HttpGet("patient{patientId}")]
        public async Task<ActionResult<List<MedicalConditionDTO>>> GetAllMedicalConditions(Guid patientId)
        {
            var query = new GetAllMedicalConditionsQuery(patientId);
            var medicalConditions = await mediator.Send(query);
            return Ok(medicalConditions);
        }

        [HttpGet("patient/{patientId}/condition/{medicalConditionId}")]
        public async Task<IActionResult> GetMedicalConditionForPatient(Guid patientId, Guid medicalConditionId)
        {
            var query = new GetMedicalConditionByIdQuery(patientId, medicalConditionId);
            var medicalCondition = await mediator.Send(query);

            if (medicalCondition == null)
                return NotFound();

            return Ok(medicalCondition);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalCondition(Guid id)
        {
           await mediator.Send(new DeleteMedicalConditionByIdCommand { Id = id });
           return NoContent();
        }

        [HttpPut("{medicalConditionId}")]
        public async Task<IActionResult> UpdateMedicalCondition(Guid medicalConditionId, UpdateMedicalConditionCommand command)
        {
            if (medicalConditionId != command.MedicalConditionId)
            {
                return BadRequest();
            }

            var result = await mediator.Send(command);
            if (result == null)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
