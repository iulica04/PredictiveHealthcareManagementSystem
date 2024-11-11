using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Queries
{
    public class GetAdminByIdQuery : IRequest<Result<AdminDto>>
    {
        public Guid Id { get; set; }
    }
}
