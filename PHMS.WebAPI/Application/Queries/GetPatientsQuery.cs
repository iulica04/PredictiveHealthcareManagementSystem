
using Application.DTOs;
using MediatR;

namespace Application.Queries
{
    public class GetPatientsQuery : IRequest<List<PatientDto>>
    {
    }
}
