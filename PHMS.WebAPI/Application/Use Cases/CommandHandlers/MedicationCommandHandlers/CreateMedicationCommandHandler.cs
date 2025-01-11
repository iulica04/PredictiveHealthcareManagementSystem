using Application.Commands.MedicationCommand;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers.MedicationCommandHandler
{
    public class CreateMedicationCommandHandler : IRequestHandler<CreateMedicationCommand, Result<Guid>>
    {
        private readonly IMedicationRepository repository;
        private readonly IMapper mapper;
        public CreateMedicationCommandHandler(IMedicationRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateMedicationCommand request, CancellationToken cancellationToken)
        {
            var medication = mapper.Map<Domain.Entities.Medication>(request);
            var result = await repository.AddAsync(medication);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
