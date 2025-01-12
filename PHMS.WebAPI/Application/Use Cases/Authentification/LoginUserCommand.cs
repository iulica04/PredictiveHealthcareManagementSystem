using Domain.Entities;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginUserCommand : IRequest<LoginResponse>
    {
<<<<<<< HEAD
        public required string Email { get; set; }
        public required string Password { get; set; }
=======
        public string Email { get; set; }
        public string Password { get; set; }
>>>>>>> origin/iulia
    }
}
