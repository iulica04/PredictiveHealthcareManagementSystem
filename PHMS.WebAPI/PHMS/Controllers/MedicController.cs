using Application.Commands.Medic;
using Application.DTOs;
using Application.Queries;
using Application.Queries.MedicQueries;
using Application.Use_Cases.Authentification;
using Application.Utils;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MedicController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;

        public MedicController(IMediator mediator, IConfiguration configuration)
        {
            this.mediator = mediator;
            this.configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> CreateMedic(CreateMedicCommand command)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);
            command.Password = hashedPassword;
            var id = await mediator.Send(command);
            return CreatedAtAction("GetByID", new { Id = id.Data }, id.Data);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginMedic(LoginUserCommand command)
        {
            var response = await mediator.Send(command);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<MedicDto>>> GetAllMedics()
        {
            return await mediator.Send(new GetAllMedicsQuery());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await mediator.Send(new GetMedicByIdQuery { Id = id });
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedic(Guid id, UpdateMedicCommand command)
        {
            // Extract the Authorization header
            var authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }

            var token = authHeader.Replace("Bearer ", "");
            
            var medicId = ExtractNameFromToken(token, configuration["Jwt:Key"]!);
            var requesterRole = ExtractRoleFromToken(token, configuration["Jwt:Key"]!);
            if (medicId == null)
            {
                return Unauthorized("Invalid or expired token");
            }

            if (medicId != id.ToString() && requesterRole != "Admin")
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
        public async Task<IActionResult> DeleteMedic(Guid id)
        {

            // Extract the Authorization header
            var authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }

            var token = authHeader.Replace("Bearer ", "");

            var medicId = ExtractNameFromToken(token, configuration["Jwt:Key"]!);
            var requesterRole = ExtractRoleFromToken(token, configuration["Jwt:Key"]!);
            if (medicId == null)
            {
                return Unauthorized("Invalid or expired token");
            }

            if (medicId != id.ToString() && requesterRole != "Admin")
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

        [HttpGet("paginated")]
        public async Task<ActionResult<PagedResult<MedicDto>>> GetPaginatedMedics([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? rank, [FromQuery] string? specialization)
        {
            Expression<Func<Medic, bool>> filter = m =>
               (string.IsNullOrEmpty(rank) || m.Rank == rank) &&
               (string.IsNullOrEmpty(specialization) || m.Specialization == specialization);

            var query = new GetFilteredQuery<Medic, MedicDto>
            {
                Page = page,
                PageSize = pageSize,
                Filter = filter
            };

            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
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


        public static string? ExtractRoleFromToken(string token, string secretKey)
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
                return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            }
            catch
            {
                return null; // Return null if validation or claim extraction fails
            }
        }
    }
}
