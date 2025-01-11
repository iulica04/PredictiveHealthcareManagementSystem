using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Commands.Administrator;
using Application.Queries.AdminQueries;
using Application.Use_Cases.Authentification;
using Domain.Entities;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;

        public AdminController(IMediator mediator, IConfiguration configuration)
        {
            this.mediator = mediator;
            this.configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginAdmin(LoginUserCommand command)
        {
            var response = await mediator.Send(command);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminDto>>> GetAll()
        {
            var admins = await mediator.Send(new GetAllAdminsQuery());
            return Ok(admins);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await mediator.Send(new GetAdminByIdQuery { Id = id });
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateAdminCommand command)
        {
            var authHeader = Request.Headers.Authorization.ToString();
            var authStatus = IAuthorizationManager.EnsureProperAuthorization(authHeader, configuration["Jwt:Key"]!, id, []);
            if (!authStatus.IsSuccess)
            {
                return Unauthorized(authStatus.ErrorMessage);
            }

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
        public async Task<IActionResult> Delete(Guid id)
        {
            var authHeader = Request.Headers.Authorization.ToString();
            var authStatus = IAuthorizationManager.EnsureProperAuthorization(authHeader, configuration["Jwt:Key"]!, id, []);
            if (!authStatus.IsSuccess)
            {
                return Unauthorized(authStatus.ErrorMessage);
            }

            var result = await mediator.Send(new DeleteAdminByIdCommand(id));
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }
    }
}
