using Application.Commands.Medic;
using Application.DTOs;
using Application.Queries;
using Application.Queries.MedicQueries;
using Application.Use_Cases.Authentification;
using Application.Utils;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

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
        public async Task<IActionResult> CreateMedic(CreateMedicCommand command)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);
            command.Password = hashedPassword;
            var id = await mediator.Send(command);
            return CreatedAtAction("GetByID", new { Id = id.Data }, id.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginMedic(LoginMedicCommand command)
        {
            var token = await mediator.Send(command);
            return Ok(new { Token = token });
        }

        [HttpGet]
        public async Task<ActionResult<List<MedicDto>>> GetAllMedics()
        {
            return await mediator.Send(new GetAllMedicsQuery());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await mediator.Send(new GetMedicByIdQuery { Id = id });
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedic(Guid id, UpdateMedicCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return NotFound(result.ErrorMessage);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedic(Guid id)
        {
            var result = await mediator.Send(new DeleteMedicByIdCommand(id));
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);

        }

        [HttpGet("paginated")]
        public async Task<ActionResult<PagedResult<MedicDto>>> GetPaginatedMedics([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? rank, [FromQuery] string? specialization)
        {
            Expression<Func<Medic, bool>> filter = m =>
               (string.IsNullOrEmpty(rank) || m.Rank == rank) &&
               (string.IsNullOrEmpty(specialization) || m.Specialization == specialization);

            var query = new GetFilteredQuery<Medic, MedicDto>
            {
                Page = page,
                PageSize = pageSize,
                Filter = filter
            };

            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }
    }
}
