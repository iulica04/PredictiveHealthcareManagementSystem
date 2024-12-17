using Application.Use_Cases.Authentification;
using Application.Use_Cases.Commands.UserCommands;
using Application.Use_Cases.ResetPassword;
using Domain.Common;
using Domain.Entities;
using Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PHMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly IValidationTokenService validationTokenService;

        public AuthController(IMediator mediator, IConfiguration configuration, IEmailService emailService, IValidationTokenService validationTokenService)
        {
            this.mediator = mediator;
            this.configuration = configuration;
            this.emailService = emailService;
            this.validationTokenService = validationTokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Result<LoginResponse>>> Login(LoginCommand command)
        {
            var response = await mediator.Send(command);
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessage);
            }
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<Result<Guid>>> Register(RegisterCommand command)
        {
            var response = await mediator.Send(command);
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessage);
            }
            return StatusCode(201, response);
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
