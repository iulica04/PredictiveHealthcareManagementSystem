using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginCommand : IRequest<Result<LoginResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
