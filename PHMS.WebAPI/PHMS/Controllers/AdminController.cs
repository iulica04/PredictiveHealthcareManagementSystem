using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Commands.Administrator;
using Application.Queries.AdminQueries;
using Application.Use_Cases.Authentification;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Commands.Medic;

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
        public async Task<IActionResult> LoginAdmin(LoginUserCommand command)
        {
            var token = await mediator.Send(command);
            return Ok(new { Token = token });
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
            // Extract the Authorization header
            var authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }

            var token = authHeader.Replace("Bearer ", "");

            var medicId = ExtractNameFromToken(token, configuration["Jwt:Key"]!);
            if (medicId == null)
            {
                return Unauthorized("Invalid or expired token");
            }

            if (medicId != id.ToString())
            {
                return Unauthorized("You are not authorized to update this medic");
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
            // Extract the Authorization header
            var authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }

            var token = authHeader.Replace("Bearer ", "");

            var medicId = ExtractNameFromToken(token, configuration["Jwt:Key"]!);
            if (medicId == null)
            {
                return Unauthorized("Invalid or expired token");
            }

            if (medicId != id.ToString())
            {
                return Unauthorized("You are not authorized to update this medic");
            }
            var result = await mediator.Send(new DeleteMedicByIdCommand(id));
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }

        public static string? ExtractNameFromToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            }
            catch
            {
                return null; // Return null if validation or claim extraction fails
            }
        }
    }
}
