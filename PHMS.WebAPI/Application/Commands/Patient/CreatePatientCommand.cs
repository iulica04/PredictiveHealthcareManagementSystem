using Domain.Common;
using MediatR;

namespace Application.Commands.Patient
{
    public class CreatePatientCommand : IRequest<Result<Guid>>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required string Address { get; set; }
    }
}
