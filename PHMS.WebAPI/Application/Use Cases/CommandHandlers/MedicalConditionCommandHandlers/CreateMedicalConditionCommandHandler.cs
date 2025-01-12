using Application.Commands.MedicalConditionCommands;
using Domain.Entities;
using AutoMapper;
using Domain.Common;
using MediatR;
using Domain.Repositories;

namespace Application.CommandHandlers.MedicalConditionCommandHandlers
{
    public class CreateMedicalConditionCommandHandler : IRequestHandler<CreateMedicalConditionCommand, Result<Guid>>
    {
        private readonly IMedicalConditionRepository repository;
        private readonly ITreatmentRepository treatmentRepository;
        private readonly IMapper mapper;

        public CreateMedicalConditionCommandHandler(IMedicalConditionRepository repository, IMapper mapper, ITreatmentRepository treatmentRepository)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.treatmentRepository = treatmentRepository;
        }

        public async Task<Result<Guid>> Handle(CreateMedicalConditionCommand request, CancellationToken cancellationToken)
        {
            var medicalCondition = mapper.Map<MedicalCondition>(request);

            // Map treatments from request to medicalCondition
            medicalCondition.Treatments = request.Treatments;

            foreach (var treatment in request.Treatments)
            {
                var existingTreatment = await treatmentRepository.GetByNameAsync(treatment.Name);
                if (existingTreatment == null)
                {
                    await treatmentRepository.AddAsync(treatment);
                }
                else
                {
                    treatment.TreatmentId = existingTreatment.TreatmentId;
                }
            }

            var result = await repository.AddAsync(medicalCondition);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}