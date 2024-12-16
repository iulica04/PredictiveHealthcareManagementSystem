using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class RegisterUserCommand : IRequest<Result<Guid>>
    {
        public required UserType Type { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
    }
}
