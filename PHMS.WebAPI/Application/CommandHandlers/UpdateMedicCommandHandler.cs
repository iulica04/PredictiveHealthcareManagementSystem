using AutoMapper;
using Domain.Repositories;
using MediatR;
using Domain.Common;
using Application.Commands.Medic;

namespace Application.CommandHandlers
{
    public class UpdateMedicCommandHandler : IRequestHandler<UpdateMedicCommand, Result<Unit>>
    {
        private readonly IMedicRepository repository;
        private readonly IMapper mapper;

        public UpdateMedicCommandHandler(IMedicRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(UpdateMedicCommand request, CancellationToken cancellationToken)
        {
            var medic = await repository.GetByIdAsync(request.Id);
            if (medic == null)
            {
                return Result<Unit>.Failure("Medic not found");
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                medic.PasswordHash = PasswordHasher.HashPassword(request.Password);
            }

            // Map other properties from request to medic
            mapper.Map(request, medic);

            await repository.UpdateAsync(medic);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}