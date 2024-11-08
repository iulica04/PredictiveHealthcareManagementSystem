using Domain.Common;
using MediatR;

namespace Application.Commands.Medic
{
    public class DeleteMedicCommandById : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }
}
