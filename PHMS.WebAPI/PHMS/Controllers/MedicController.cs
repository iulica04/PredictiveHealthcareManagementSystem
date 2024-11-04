using Application.Commands;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MedicController : ControllerBase
    {
        private readonly IMediator mediator;

        public MedicController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMedic(CreateMedicCommand command)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);
            command.Password = hashedPassword;
            var id = await mediator.Send(command);
            return CreatedAtAction("GetByID", new { Id = id }, id);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var medic = await mediator.Send(new GetMedicByIdQuery { Id = id });
            if (medic == null)
            {
                return NotFound();
            }
            return Ok(medic);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedic(Guid id)
        {
            await mediator.Send(new DeleteMedicCommandById { Id = id });
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedic(Guid id, UpdateMedicCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            await mediator.Send(command);
            return NoContent();
        }
    }
}
