using Application.DTOs;
using MediatR;

namespace Application.Queries.PrescriptionQueries
{
    public class GetAllPrescriptionsQuery : IRequest<List<PrescriptionDto>>
    {
    }
}
