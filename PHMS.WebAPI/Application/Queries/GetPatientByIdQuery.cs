using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Queries
{
    public class GetPatientByIdQuery : IRequest<Result<PatientDto>>
    {
        public Guid Id { get; set; }
    }
}
