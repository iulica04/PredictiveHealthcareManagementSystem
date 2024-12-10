using Application.Commands.Patient;
using Application.DTOs;
using Application.Queries.PatientQueries;
using Application.Use_Cases.Authentification;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;
        public PatientController(IMediator mediator, IConfiguration configuration)
        {
            this.mediator = mediator;
            this.configuration = configuration; 
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreatePatient(CreatePatientCommand command)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);
            command.Password = hashedPassword;
            var result = await mediator.Send(command);
            return CreatedAtAction(nameof(GetByID), new { Id = result.Data }, result.Data);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginPatient(LoginUserCommand command)
        {
            var token = await mediator.Send(command);
            return Ok(new { Token = token });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await mediator.Send(new GetPatientByIdQuery { Id = id });
            if(result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
           var patients= await mediator.Send(new GetAllPatientsQuery());
           return Ok(patients);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdatePatientCommand command)
        {
            var authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }

            var token = authHeader.Replace("Bearer ", "");

            var patientId = ExtractNameFromToken(token, configuration["Jwt:Key"]!);
            if (patientId == null)
            {
                return Unauthorized("Invalid or expired token");
            }

            if (patientId != id.ToString())
            {
                return Unauthorized("You are not authorized to update this patient");
            }
            if (id != command.Id)
            {
                return BadRequest("The id should be identical with command.Id");
            }


            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await mediator.Send(new DeletePatientByIdCommand(id));
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }

        //[HttpGet("paginated")]
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
