using Application.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers
{
    public class CreateMedicCommandHandler : IRequestHandler<CreateMedicCommand, Guid>
    {
        private readonly IMedicRepository repository;
        private readonly IMapper mapper;

        public CreateMedicCommandHandler(IMedicRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Guid> Handle(CreateMedicCommand request, CancellationToken cancellationToken)
        {

            var medic = mapper.Map<Medic>(request);
            medic.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            return await repository.AddAsync(medic);
        }
    }
}
