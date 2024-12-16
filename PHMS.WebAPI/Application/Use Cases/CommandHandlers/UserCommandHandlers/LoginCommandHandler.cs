using Application.Utils;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IUserRepository UserRepository;

        public LoginCommandHandler(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var hashedPassword = PasswordHasher.HashPassword(request.Password);
            return await UserRepository.Login(request.Email, hashedPassword);
        }
    }
}
