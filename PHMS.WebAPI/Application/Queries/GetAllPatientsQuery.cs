
using Application.DTOs;
using MediatR;

namespace Application.Queries
{
    public class GetAllPatientsQuery : IRequest<List<PatientDto>>
    {
    }
}
