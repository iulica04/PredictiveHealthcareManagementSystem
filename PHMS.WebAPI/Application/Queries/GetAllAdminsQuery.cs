using Application.DTOs;
using MediatR;

namespace Application.Queries
{
    public class GetAllAdminsQuery : IRequest<List<AdminDto>>
    {
    }
}
