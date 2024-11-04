using Application.DTOs;
using MediatR;

namespace Application.Queries
{
    public class GetPatientByIdQuery : IRequest<PatientDto>
    {
        public Guid Id { get; set; }
    }
}
