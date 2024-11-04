using Application.DTOs;
using Application.Queries;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.QueryHandlers
{
    public class GetMedicByIdQueryHandler : IRequestHandler<GetMedicByIdQuery, MedicDto>
    {
        private readonly IPatientRepository repository;
        private readonly IMapper mapper;
        public GetMedicByIdQueryHandler(IPatientRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<MedicDto> Handle(GetMedicByIdQuery request, CancellationToken cancellationToken)
        {
            var medic = await repository.GetByIdAsync(request.Id);
            return mapper.Map<MedicDto>(medic);
        }
    }
}
