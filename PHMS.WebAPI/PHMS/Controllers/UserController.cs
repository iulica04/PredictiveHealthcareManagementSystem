using Application.Use_Cases.Queries.UserQueries;
using Domain.Common;
using Domain.Entities;
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
        public async Task<ActionResult<Result<User>>> GetAllOfType([FromQuery] GetUsersOfTypeQuery query)
        {
            var response = await mediator.Send(query);
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessage);
            }
            return Ok(response);
        }
    }
}
