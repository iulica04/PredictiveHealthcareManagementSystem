using Application.DTOs;
using Application.Queries;
using Application.Use_Cases.Commands.UserCommands;
using Application.Use_Cases.Queries.UserQueries;
using Application.Utils;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

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

        [HttpPut("update/{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Id in the body does not match the id in the route");
            }
            var response = await mediator.Send(command);
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessage);
            }
            return Ok(response);
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<PagedResult<UserDto>>> GetPaginatedUsers([FromQuery] GetFilteredUsersQuery query)
        {
            Expression<Func<User, bool>> filter = user =>
                (user.Type != UserType.Medic) ||
                (string.IsNullOrEmpty(query.Rank) || ((Medic)user).Rank == query.Rank) &&
                (string.IsNullOrEmpty(query.Specialization) || ((Medic)user).Specialization == query.Specialization) &&
                (string.IsNullOrEmpty(query.Hospital) || ((Medic)user).Hospital == query.Hospital);

            var funQuery = new GetFilteredQuery<User, UserDto>
            {
                Type = query.Type,
                Page = query.Page,
                PageSize = query.PageSize,
                Filter = filter
            };

            var result = await mediator.Send(funQuery);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }
    }
}
