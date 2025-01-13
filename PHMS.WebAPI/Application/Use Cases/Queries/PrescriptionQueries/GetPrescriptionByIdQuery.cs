using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Queries.PrescriptionQueries
{
    public class GetPrescriptionByIdQuery : IRequest<Result<PrescriptionDto>>
    {
        public Guid Id { get; set; }
    }
}
