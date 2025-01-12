using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Use_Cases.Commands.TreatmentCommands
{
    public abstract class TreatmentCommand<T>: IRequest<Result<T>>
    {
        public required string Type { get; set; }
        public Prescription? Prescription { get; set; }
        public required string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Duration { get; set; }
        public required string Frequency { get; set; }
    }
}
