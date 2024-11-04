using Application.Commands;
using Application.Queries;
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
        public async Task<IActionResult> CreatePatient(CreatePatientCommand command)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);
            command.Password = hashedPassword;
            var id = await mediator.Send(command);
            return CreatedAtAction("GetByID", new { Id = id }, id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            // Logic to retrieve the patient by ID
            var patient = await mediator.Send(new GetPatientByIdQuery { Id = id });
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }
    }
}
