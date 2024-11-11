using Domain.Common;
using MediatR;

namespace Application.Commands.Medic
{
    public class DeleteMedicByIdCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }
}
