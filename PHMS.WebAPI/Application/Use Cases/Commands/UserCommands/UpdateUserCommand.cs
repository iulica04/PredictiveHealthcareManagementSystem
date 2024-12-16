using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Use_Cases.Commands.UserCommands
{
    public class UpdateUserCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
        public required UserType Type { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required string Address { get; set; }

        public List<PatientRecord>? PatientRecords { get; set; }

        public string? Rank { get; set; }
        public string? Specialization { get; set; }
        public string? Hospital { get; set; }
    }
}
