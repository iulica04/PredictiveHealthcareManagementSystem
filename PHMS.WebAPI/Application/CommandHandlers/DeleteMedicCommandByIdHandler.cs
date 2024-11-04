using Application.Commands;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers
{
    public class DeleteMedicCommandByIdHandler : IRequestHandler<DeleteMedicCommandById>
    {
        private readonly IMedicRepository repository;

        public DeleteMedicCommandByIdHandler(IMedicRepository repository)
        {
            this.repository = repository;
        }

        public async Task Handle(DeleteMedicCommandById request, CancellationToken cancellationToken)
        {
            await repository.DeleteAsync(request.Id);
        }
    }
}
