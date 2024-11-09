using Domain.Common;
using MediatR;

namespace Application.Commands
{
    public class DeleteMedicByIdCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }
}
