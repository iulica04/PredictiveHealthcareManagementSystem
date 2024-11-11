using Application.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Queries
{
    public class GetAllAdminsQuery : IRequest<List<AdminDto>>
    {
    }
}
