using Application.Commands.Patient;
using Application.DTOs;
using Application.Queries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IMediator mediator;
        public PatientController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreatePatient(CreatePatientCommand command)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);
            command.Password = hashedPassword;
            var result = await mediator.Send(command);
            return CreatedAtAction(nameof(GetByID), new { Id = result.Data }, result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await mediator.Send(new GetPatientByIdQuery { Id = id });
            if(result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
           var patients= await mediator.Send(new GetAllPatientsQuery());
           return Ok(patients);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdatePatientCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("The id should be identical with command.Id");
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
            var result = await mediator.Send(new DeletePatientByIdCommand(id));
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }
    }
}
