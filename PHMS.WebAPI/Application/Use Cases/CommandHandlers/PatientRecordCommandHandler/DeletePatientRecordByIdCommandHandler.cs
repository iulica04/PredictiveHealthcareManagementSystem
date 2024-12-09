using Application.Commands.PatientRecordByIdCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers.PatientRecordCommandHandler
{
    public class DeletePatientRecordByIdCommandHandler : IRequestHandler<DeletePatientRecordByIdCommand, Result<Unit>>
    {
        private readonly IPatientRecordRepository repository;
        public DeletePatientRecordByIdCommandHandler(IPatientRecordRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Result<Unit>> Handle(DeletePatientRecordByIdCommand request, CancellationToken cancellationToken)
        {
            var patientRecord = await repository.GetByIdAsync(request.Id);
            if (patientRecord == null)
            {
                return Result<Unit>.Failure("Patient Record not found");
            }
            await repository.DeleteAsync(request.Id);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
