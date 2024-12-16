using Application.Utils;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginResponse>>
    {
        private readonly IUserRepository UserRepository;

        public LoginUserCommandHandler(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public async Task<Result<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var hashedPassword = PasswordHasher.HashPassword(request.Password);
            return await UserRepository.Login(request.Email, hashedPassword);
        }
    }
}
