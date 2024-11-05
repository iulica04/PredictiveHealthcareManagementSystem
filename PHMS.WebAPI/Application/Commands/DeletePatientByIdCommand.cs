using Domain.Common;
using MediatR;

namespace Application.Commands
{
    public class DeletePatientByIdCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }
}
