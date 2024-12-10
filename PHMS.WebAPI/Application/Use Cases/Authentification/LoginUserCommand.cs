using Domain.Entities;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginUserCommand : IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
