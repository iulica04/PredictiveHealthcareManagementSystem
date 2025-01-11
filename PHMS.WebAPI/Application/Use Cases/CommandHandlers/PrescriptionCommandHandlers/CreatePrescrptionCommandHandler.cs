using Application.Commands.PrescriptionCommandHandler;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers.PrescriptionCommandHandlers
{
    public class CreatePrescrptionCommandHandler : IRequestHandler<CreatePrescriptionCommand, Result<Guid>>
    {
        private readonly IPrescriptionRepository repository;
        private readonly IMapper mapper;
        public CreatePrescrptionCommandHandler(IPrescriptionRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
        {
            var prescription = mapper.Map<Prescription>(request);
            var result = await repository.AddAsync(prescription);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
