using Application.DTOs;
using Application.Use_Cases.Commands.UserCommands;
using Application.Use_Cases.Queries.UserQueries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;
        public UserController(IMediator mediator, IConfiguration configuration)
        {
            this.mediator = mediator;
            this.configuration = configuration;
        }

        [HttpGet("get")]
        public async Task<ActionResult<Result<IEnumerable<UserDto>>>> GetAllOfType([FromQuery] GetUsersOfTypeQuery query)
        {
            var response = await mediator.Send(query);
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessage);
            }
            return Ok(response);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Result<UserDto?>>> GetById(Guid id)
        {
            var response = await mediator.Send(new GetUserByIdQuery { Id = id } );
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessage);
            }
            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var response = await mediator.Send(new DeleteUserCommand { Id = id });
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessage);
            }
            return Ok(response);
        }
    }
}
