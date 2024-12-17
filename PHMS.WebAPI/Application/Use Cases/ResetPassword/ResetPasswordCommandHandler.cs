using Application.Utils;
using Domain.Common;
using Domain.Repositories;
using Domain.Services;
using MediatR;

namespace Application.Use_Cases.ResetPassword
{
    internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<Unit>>
    {
        private readonly IUserRepository repository;
        private readonly IValidationTokenService tokenService;

        public ResetPasswordCommandHandler(IUserRepository repository, IValidationTokenService tokenService)
        {
            this.repository = repository;
            this.tokenService = tokenService;
        }
        public async Task<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {

            if (!await tokenService.ValidateResetTokenAsync(request.Email, request.Token))
            {
                return Result<Unit>.Failure("Invalid token");
            }

            var user = await repository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<Unit>.Failure("Email not found");
            }

            user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
            await repository.UpdateUserAsync(user);
            await tokenService.DeleteResetTokenAsync(request.Email);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
