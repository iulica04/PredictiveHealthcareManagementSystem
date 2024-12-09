using Application.DTOs;
using Application.Queries.PatientRecordQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.QueryHandlers.PatientRecordQueryHandlers
{
    public class GetPatientRecordByIdQueryHandler : IRequestHandler<GetPatientRecordByIdQuery, Result<PatientRecordDto>>
    {
        private readonly IPatientRecordRepository patientRecordRepository;
        private readonly IMapper mapper;
        public GetPatientRecordByIdQueryHandler(IPatientRecordRepository patientRecordRepository, IMapper mapper)
        {
            this.patientRecordRepository = patientRecordRepository;
            this.mapper = mapper;
        }
        public async Task<Result<PatientRecordDto>> Handle(GetPatientRecordByIdQuery request, CancellationToken cancellationToken)
        {
            var patientRecord = await patientRecordRepository.GetByIdAsync(request.PatientRecordId);
            if (patientRecord == null)
            {
                return Result<PatientRecordDto>.Failure("Patient Record not found");
            }
            return Result<PatientRecordDto>.Success(mapper.Map<PatientRecordDto>(patientRecord));
        }
    }
}
