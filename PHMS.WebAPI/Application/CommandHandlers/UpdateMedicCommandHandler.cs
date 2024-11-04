using Application.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers
{
    public class UpdateMedicCommandHandler : IRequestHandler<UpdateMedicCommand>
    {
        private readonly IMedicRepository repository;
        private readonly IMapper mapper;

        public UpdateMedicCommandHandler(IMedicRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public Task Handle(UpdateMedicCommand request, CancellationToken cancellationToken)
        {
            var medic = mapper.Map<Medic>(request);
            medic.Id = request.Id;
            return repository.UpdateAsync(medic);
        }
    }
}
