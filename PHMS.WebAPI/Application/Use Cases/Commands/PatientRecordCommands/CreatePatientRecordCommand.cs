using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Commands.PatientRecordCommands
{
    public class CreatePatientRecordCommand : IRequest<Result<Guid>>
    {
        public Guid PatientId { get; set; }
        public required MedicalCondition MedicalCondition { get; set; }
        public required Treatment Treatment { get; set; }
    }
}
