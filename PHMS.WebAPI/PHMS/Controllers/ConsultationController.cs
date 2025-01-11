using Application.DTOs;
using Application.Use_Cases.Commands.ConsultationCommands;
using Domain.Common;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PHMS.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;

        public ConsultationController(IMediator mediator, IConfiguration configuration)
        {
            this.mediator = mediator;
            this.configuration = configuration;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestConsultation([FromBody] CreateConsultationCommand command)
        {
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(new { ConsultationId = result.Data });
        }

    }
}
