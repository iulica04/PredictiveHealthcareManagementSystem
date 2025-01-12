using Domain.Common;
using MediatR;

namespace Application.Commands.PatientRecordByIdCommands
{
    public record DeletePatientRecordByIdCommand(Guid Id) : IRequest<Result<Unit>>;
}
