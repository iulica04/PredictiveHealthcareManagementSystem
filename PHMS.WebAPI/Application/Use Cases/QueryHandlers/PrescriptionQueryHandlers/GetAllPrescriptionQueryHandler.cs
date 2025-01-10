using Application.DTOs;
using Application.Queries.PrescriptionQueries;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.QueryHandlers.PrescriptionQueryHandlers
{
    public class GetAllPrescriptionQueryHandler : IRequestHandler<GetAllPrescriptionsQuery, List<PrescriptionDto>>
    {
        private readonly IPrescriptionRepository prescriptionRepository;
        private readonly IMapper mapper;

        public GetAllPrescriptionQueryHandler(IPrescriptionRepository prescriptionRepository, IMapper mapper)
        {
            this.prescriptionRepository = prescriptionRepository;
            this.mapper = mapper;
        }
        public async Task<List<PrescriptionDto>> Handle(GetAllPrescriptionsQuery request, CancellationToken cancellationToken)
        {
            var prescriptions = await prescriptionRepository.GetAllAsync();
            return mapper.Map<List<PrescriptionDto>>(prescriptions);
        }
    }
}
