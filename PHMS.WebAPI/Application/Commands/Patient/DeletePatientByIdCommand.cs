using Domain.Common;
using MediatR;

namespace Application.Commands.Patient
{
    public class DeletePatientByIdCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }
}
