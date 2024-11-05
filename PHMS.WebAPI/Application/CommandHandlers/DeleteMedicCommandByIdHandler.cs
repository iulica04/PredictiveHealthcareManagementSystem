using Application.Commands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers
{
    public class DeleteMedicCommandByIdHandler : IRequestHandler<DeleteMedicCommandById, Result<Unit>>
    {
        private readonly IMedicRepository repository;

        public DeleteMedicCommandByIdHandler(IMedicRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<Unit>> Handle(DeleteMedicCommandById request, CancellationToken cancellationToken)
        {
            var medic = await repository.GetByIdAsync(request.Id);
            if (medic == null)
            {
                return Result<Unit>.Failure("Medic not found");
            }
            await repository.DeleteAsync(request.Id);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
