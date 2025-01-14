using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Use_Cases.Commands.ConsultationCommands
{
    public abstract class ConsultationCommand<T> : IRequest<Result<T>>
    {
        public required Guid PatientId { get; set; }
        public required Guid MedicId { get; set; }
        public ConsultationStatus Status { get; set; } = ConsultationStatus.Pending;
        public required DateTime Date { get; set; }
        public required string Location { get; set; }
    }
}
