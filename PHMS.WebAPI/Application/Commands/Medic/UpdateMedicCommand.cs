using MediatR;
using Domain.Common;

namespace Application.Commands.Medic
{
    public class UpdateMedicCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required string Address { get; set; }
        public required string Rank { get; set; }
        public required string Specialization { get; set; }
        public required string Hospital { get; set; }
    }
}