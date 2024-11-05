using MediatR;

namespace Application.Commands
{
    public class CreateMedicCommand : IRequest<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Rank { get; set; }
        public string Specialization { get; set; }
        public string Hospital { get; set; }
    }
}
