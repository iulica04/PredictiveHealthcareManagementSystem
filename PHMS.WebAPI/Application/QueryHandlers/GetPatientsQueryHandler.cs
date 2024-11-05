
using Application.DTOs;
using Application.Queries;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.QueryHandlers
{
    public class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, List<PatientDto>>
    {
        private readonly IPatientRepository patientRepository;
        private readonly IMapper mapper;

        public GetPatientsQueryHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            this.patientRepository = patientRepository;
            this.mapper = mapper;
        }

        public async Task<List<PatientDto>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
        {
            var patients = await patientRepository.GetAllAsync();
            return mapper.Map<List<PatientDto>>(patients);
        }

    }
}
