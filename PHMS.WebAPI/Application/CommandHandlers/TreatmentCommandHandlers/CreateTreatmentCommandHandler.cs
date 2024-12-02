using Application.Commands.TreatmentCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers.TreatmentCommandHandler
{
    public class CreateTreatmentCommandHandler : IRequestHandler<CreateTreatmentCommand, Result<Guid>>
    {
        private readonly ITreatmentRepository repository;
        private readonly IMapper mapper;
        
        public CreateTreatmentCommandHandler(ITreatmentRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(CreateTreatmentCommand request, CancellationToken cancellationToken)
        {
            var treatment = mapper.Map<Treatment>(request);
            var result = await repository.AddAsync(treatment);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
