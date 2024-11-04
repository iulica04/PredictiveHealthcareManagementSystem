using MediatR;

namespace Application.Commands
{
    public class UpdateMedicCommand : CreateMedicCommand, IRequest
    {
        public Guid Id { get; set; }
    }
}
