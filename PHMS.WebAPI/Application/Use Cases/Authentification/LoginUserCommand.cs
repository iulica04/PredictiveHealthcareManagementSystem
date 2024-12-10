using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginUserCommand : IRequest<string>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
