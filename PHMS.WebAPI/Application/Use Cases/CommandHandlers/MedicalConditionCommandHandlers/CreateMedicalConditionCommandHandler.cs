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
        private readonly IMapper mapper;

        public CreateMedicalConditionCommandHandler(IMedicalConditionRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(CreateMedicalConditionCommand request, CancellationToken cancellationToken)
        {
            var medicalCondition = mapper.Map<MedicalCondition>(request);
            var result = await repository.AddAsync(medicalCondition);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
