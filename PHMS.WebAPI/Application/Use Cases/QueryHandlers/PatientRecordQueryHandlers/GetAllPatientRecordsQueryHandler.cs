using Application.DTOs;
using Application.Queries.PatientRecordQueries;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.QueryHandlers.PatientRecordQueryHandlers
{
    public class GetAllPatientRecordsQueryHandler : IRequestHandler<GetAllPatientRecordsQuery, List<PatientRecordDto>>
    {
        private readonly IPatientRecordRepository patientRecordRepository;
        private readonly IMapper mapper;
        public GetAllPatientRecordsQueryHandler(IPatientRecordRepository patientRecordRepository, IMapper mapper)
        {
            this.patientRecordRepository = patientRecordRepository;
            this.mapper = mapper;
        }
        public async Task<List<PatientRecordDto>> Handle(GetAllPatientRecordsQuery request, CancellationToken cancellationToken)
        {
            var patientRecords = await patientRecordRepository.GetAllAsync();
            return mapper.Map<List<PatientRecordDto>>(patientRecords);
        }
    }
}
