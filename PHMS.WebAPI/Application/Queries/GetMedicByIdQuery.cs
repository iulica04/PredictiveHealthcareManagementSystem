using Application.DTOs;
using MediatR;


namespace Application.Queries
{
    public class GetMedicByIdQuery : IRequest<MedicDto>
    {
        public Guid Id { get; set; }
    }
}
