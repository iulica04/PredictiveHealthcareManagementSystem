using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Queries.PatientRecordQueries
{
    public class GetPatientRecordByIdQuery : IRequest<Result<PatientRecordDto>>
    {
        public Guid PatientRecordId { get; set; }
    }
}
