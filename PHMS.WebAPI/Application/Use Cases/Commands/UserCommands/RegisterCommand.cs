using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Use_Cases.Commands.UserCommands
{
    public class RegisterCommand : IRequest<Result<Guid>>
    {
        public required UserType Type { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required string Address { get; set; }

        public string? Rank { get; set; }
        public string? Specialization { get; set; }
        public string? Hospital { get; set; }
    }
}
