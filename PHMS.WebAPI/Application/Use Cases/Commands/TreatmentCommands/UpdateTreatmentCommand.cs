using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.TreatmentCommands
{
    public class UpdateTreatmentCommand : IRequest<Result<Unit>>
    {
        public Guid TreatmentId { get; set; }
        public TreatmentType Type { get; set; }
        public required Prescription Prescription { get; set; }
        public required string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Duration { get; set; }
        public required string Frequency { get; set; }
    }
}
