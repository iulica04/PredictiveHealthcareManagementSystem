using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Commands.PrescriptionCommandHandler
{
    public class CreatePrescriptionCommand : IRequest<Result<Guid>>
    {
        public DateTime DateIssued { get; set; }
        public required List<Medication> Medications { get; set; }
    }
}
