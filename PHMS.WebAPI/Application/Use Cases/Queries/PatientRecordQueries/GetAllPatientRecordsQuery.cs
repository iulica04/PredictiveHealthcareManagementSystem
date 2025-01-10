using Application.DTOs;
using MediatR;

namespace Application.Queries.PatientRecordQueries
{
    public class GetAllPatientRecordsQuery : IRequest<List<PatientRecordDto>>
    {
    }
}
