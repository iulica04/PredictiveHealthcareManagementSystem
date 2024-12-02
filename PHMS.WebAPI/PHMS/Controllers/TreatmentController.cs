using Application.Commands.TreatmentCommands;
using Application.DTOs;
using Application.Queries.TreatmentQueries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TreatmentController : ControllerBase
    {
        private readonly IMediator mediator;
        public TreatmentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateTreatment(CreateTreatmentCommand command)
        {
            var result = await mediator.Send(command);
            return CreatedAtAction(nameof(GetByID), new { Id = result.Data }, result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await mediator.Send(new GetTreatmentByIdQuery { TreatmentId = id });
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TreatmentDto>>> GetAll()
        {
            var treatments = await mediator.Send(new GetAllTreatmentsQuery());
            return Ok(treatments);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTreatmentCommand command)
        {
            if (id != command.TreatmentId)
            {
                return BadRequest("The id should be identical with command.TreatmentId");
            }

            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await mediator.Send(new DeleteTreatmentByIdCommand(id));
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }


    }
}
