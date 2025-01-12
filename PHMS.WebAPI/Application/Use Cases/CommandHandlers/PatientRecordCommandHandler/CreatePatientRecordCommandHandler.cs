using Application.Commands.PatientRecordCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers.PatientRecordCommandHandler
{
    public class CreatePatientRecordCommandHandler : IRequestHandler<CreatePatientRecordCommand, Result<Guid>>
    {
        private readonly IPatientRecordRepository repository;
        private readonly IMapper mapper;
        public CreatePatientRecordCommandHandler(IPatientRecordRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreatePatientRecordCommand request, CancellationToken cancellationToken)
        {
           var patientRecord = mapper.Map<PatientRecord>(request);
            var result = await repository.AddAsync(patientRecord);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);

        }
    }
}
