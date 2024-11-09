using Application.DTOs;
using Domain.Common;
using MediatR;


namespace Application.Queries
{
    public class GetMedicByIdQuery : IRequest<Result<MedicDto>>
    {
        public Guid Id { get; set; }
    }
}
