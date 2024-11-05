using Application.DTOs;
using MediatR;

namespace Application.Queries
{
    public class GetAllMedicsQuery : IRequest<List<MedicDto>>
    {
    }
}
