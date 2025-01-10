using Application.Commands.MedicationCommand;
using Application.DTOs;
using Application.Queries.MedicationQueries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MedicationController : ControllerBase
    {
        private readonly IMediator mediator;
        public MedicationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateMedication(CreateMedicationCommand command)
        {
            var result = await mediator.Send(command);
            return CreatedAtAction("GetByID", new { Id = result.Data }, result.Data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await mediator.Send(new GetMedicationByIdQuery { MedicationId = id });
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicationDto>>> GetAll()
        {
            var medications = await mediator.Send(new GetAllMedicationsQuery());
            return Ok(medications);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateMedicationCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("The id should be identical with command.MedicationId");
            }

            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await mediator.Send(new DeleteMedicationByIdCommand(id));
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }
    }
}
