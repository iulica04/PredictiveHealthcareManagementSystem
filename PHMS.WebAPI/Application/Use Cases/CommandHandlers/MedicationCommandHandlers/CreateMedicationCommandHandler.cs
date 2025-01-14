using Application.Commands.MedicalConditionCommands;
using Application.Commands.TreatmentCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers.MedicalConditionCommandHandler
{
    public class CreateMedicalConditionCommandHandler : IRequestHandler<CreateMedicalConditionCommand, Result<Guid>>
    {
        private readonly IMedicalConditionRepository medicalConditionRepository;
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public CreateMedicalConditionCommandHandler(IMedicalConditionRepository medicalConditionRepository, IMediator mediator, IMapper mapper)
        {
            this.medicalConditionRepository = medicalConditionRepository;
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateMedicalConditionCommand request, CancellationToken cancellationToken)
        {
            var medicalCondition = mapper.Map<MedicalCondition>(request);

            var medicalConditionResult = await medicalConditionRepository.AddAsync(medicalCondition);
            if (!medicalConditionResult.IsSuccess)
            {
                return Result<Guid>.Failure(medicalConditionResult.ErrorMessage);
            }

            foreach (var treatmentDto in request.Treatments)
            {
                var createTreatmentCommand = new CreateTreatmentCommand
                {
                    MedicalConditionId = medicalConditionResult.Data,
                    Type = treatmentDto.Type,
                    Name = treatmentDto.Name,
                    Location = treatmentDto.Location,
                    StartDate = treatmentDto.StartDate,
                    Duration = treatmentDto.Duration,
                    Frequency = treatmentDto.Frequency,
                    Medications = treatmentDto.Medications
                };

                var treatmentResult = await mediator.Send(createTreatmentCommand, cancellationToken);
                if (!treatmentResult.IsSuccess)
                {
                    return Result<Guid>.Failure(treatmentResult.ErrorMessage);
                }
            }

            return Result<Guid>.Success(medicalConditionResult.Data);
        }
    }
}