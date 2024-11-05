using MediatR;

namespace Application.Commands
{
    public class DeleteMedicCommandById : IRequest
    {
        public Guid Id { get; set; }
    }
}
