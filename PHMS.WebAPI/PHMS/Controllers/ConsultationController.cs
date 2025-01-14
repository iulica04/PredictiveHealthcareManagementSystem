using Application.DTOs;
using Application.Use_Cases.Commands.ConsultationCommands;
using Application.Use_Cases.Queries.ConsultationsQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly IMediator mediator;

        public ConsultationController(IMediator mediator, IConfiguration configuration)
        {
            this.mediator = mediator;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestConsultation([FromBody] CreateConsultationCommand command)
        {
            try
            {
                var result = await mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }
                return Ok(new { ConsultationId = result.Data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetAll()
        {
            try
            {
                var result = await mediator.Send(new GetAllConsultationsQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            try
            {
                var result = await mediator.Send(new GetConsultationByIdQuery { Id = id });
                if (result.IsSuccess)
                {
                    return Ok(result.Data);
                }
                return NotFound(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateConsultationCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("The id should be identical with command.ConsultationId");
            }

            try
            {
                var result = await mediator.Send(command);
                if (result.IsSuccess)
                {
                    return NoContent();
                }
                return NotFound(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await mediator.Send(new DeleteConsultationCommand(id));
                if (result.IsSuccess)
                {
                    return NoContent();
                }
                return NotFound(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}