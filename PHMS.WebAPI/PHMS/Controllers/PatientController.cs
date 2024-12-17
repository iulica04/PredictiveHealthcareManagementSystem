using Application.Commands.Medic;
using Application.Commands.Patient;
using Application.DTOs;
using Application.Queries;
using Application.Queries.PatientQueries;
using Application.Use_Cases.Authentification;
using Application.Use_Cases.ResetPassword;
using Domain.Common;
using Domain.Entities;
using Domain.Services;
using Infrastructure.Services;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
        private readonly IEmailService emailService;
        private readonly IValidationTokenService validationTokenService;
        public PatientController(IMediator mediator, IConfiguration configuration, IEmailService emailService, IValidationTokenService validationCodeService)
        {
            this.mediator = mediator;
            this.configuration = configuration;
            this.emailService = emailService;
            this.validationTokenService = validationCodeService;
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
        public async Task<ActionResult<LoginResponse>> LoginPatient(LoginUserCommand command)
        {
            var response = await mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var authHeader = Request.Headers.Authorization.ToString();
            try
            {
                EnsureProperAuthorization(authHeader, configuration["Jwt:Key"]!, id, ["Medic, Admin"]);
                var result = await mediator.Send(new GetPatientByIdQuery { Id = id });
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
            var patients = await mediator.Send(new GetAllPatientsQuery());
            return Ok(patients);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdatePatientCommand command)
        {
            var authHeader = Request.Headers.Authorization.ToString();
            try
            {
                EnsureProperAuthorization(authHeader, configuration["Jwt:Key"]!, id, ["Admin"]);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var authHeader = Request.Headers.Authorization.ToString();
            try
            {
                EnsureProperAuthorization(authHeader, configuration["Jwt:Key"]!, id, ["Admin"]);
                var result = await mediator.Send(new DeletePatientByIdCommand(id));
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

        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var exists = await mediator.Send(new CheckEmailQuery { Email = email });
            return Ok(new { exists });
        }

        public static void EnsureProperAuthorization(string requestHeadersAuthorization, string secretKey, Guid requestId, List<string>? allowedRoles = null)
        {
            if (string.IsNullOrEmpty(requestHeadersAuthorization))
            {
                throw new Exception("Authorization header is missing");
            }

            var token = requestHeadersAuthorization.Replace("Bearer ", "");
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
                var requesterId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var requesterRole = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (requesterId is null || requesterRole is null)
                {
                    throw new Exception("Invalid or expired token");
                }

                if (requesterId != requestId.ToString() && (allowedRoles is null || !allowedRoles!.Contains(requesterRole!)))
                {
                    throw new Exception("You are not authorized to update this patient");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); // Return null if validation or claim extraction fails
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { success = false, message = "Email is required" });
            }

            var token = await validationTokenService.GenerateResetTokenAsync(email);
            var resetLink = $"http://localhost:4200/reset-password/{token}"; // Construiește URL-ul manual

            var message = $"Click the link to reset your password: {resetLink}";
            await emailService.SendEmailAsync(email, "Password Reset", message);
            return Ok(new { success = true, message = "Verification link sent to your email" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
           
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new { success = true, message = "Password reset successfully" });
            }
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }
    }
}
