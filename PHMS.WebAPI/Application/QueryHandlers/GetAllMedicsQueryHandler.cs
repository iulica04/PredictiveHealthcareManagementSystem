using Application.DTOs;
using Application.Queries;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.QueryHandlers
{
    public class GetAllMedicsQueryHandler : IRequestHandler<GetAllMedicsQuery, List<MedicDto>>
    {
        private readonly IMedicRepository repository;
        private readonly IMapper mapper;

        public GetAllMedicsQueryHandler(IMedicRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;            
        }
        public async Task<List<MedicDto>> Handle(GetAllMedicsQuery request, CancellationToken cancellationToken)
        {
            var medics = await repository.GetAllAsync();
            return mapper.Map<List<MedicDto>>(medics);
        }
    }
}
