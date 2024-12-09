using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.TreatmentCommands
{
    public class CreateTreatmentCommand : IRequest<Result<Guid>>
    {
        public required string Type { get; set; }
        public Prescription? Prescription { get; set; }
        public required string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Duration { get; set; }
        public required string Frequency { get; set; }
    }
}
