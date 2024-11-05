using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers
{
    public class UpdateMedicCommandHandler : IRequestHandler<UpdateMedicCommand, Medic>
    {
        private readonly IMedicRepository repository;
        private readonly IMapper mapper;

        public UpdateMedicCommandHandler(IMedicRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Medic> Handle(UpdateMedicCommand request, CancellationToken cancellationToken)
        {
            var medic = await repository.GetByIdAsync(request.Id);
            if (medic == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                medic.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            // Map other properties from request to medic
            mapper.Map(request, medic);

            await repository.UpdateAsync(medic);
            return medic;
        }
    }
}