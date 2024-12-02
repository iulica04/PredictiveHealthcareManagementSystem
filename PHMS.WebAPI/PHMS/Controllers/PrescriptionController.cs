using Application.Commands.PrescriptionCommandHandler;
using Application.DTOs;
using Application.Queries.PrescriptionQueries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IMediator mediator;
        public PrescriptionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreatePrescription(CreatePrescriptionCommand command)
        {
            var result = await mediator.Send(command);
            return CreatedAtAction("GetByID", new { Id = result.Data }, result.Data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await mediator.Send(new GetPrescriptionByIdQuery { Id = id });
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetAll()
        {
            var result = await mediator.Send(new GetAllPrescriptionsQuery());
            return Ok(result);
        }
    }
}
