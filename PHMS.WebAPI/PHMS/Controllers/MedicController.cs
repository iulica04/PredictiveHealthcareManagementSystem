using Application.Commands;
using Application.DTOs;
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

        [HttpGet]
        public async Task<ActionResult<List<MedicDto>>> GetAllMedics()
        {
            return await mediator.Send(new GetAllMedicsQuery());
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


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedic(Guid id, UpdateMedicCommand command)
        {
            if (id != command.Id)
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedic(Guid id)
        {
            await mediator.Send(new DeleteMedicCommandById { Id = id });
            return NoContent();
        }
    }
}
