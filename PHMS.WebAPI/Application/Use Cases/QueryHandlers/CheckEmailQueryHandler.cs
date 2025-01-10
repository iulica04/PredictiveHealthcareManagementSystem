using Application.Use_Cases.Queries;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers
{
    public class CheckEmailQueryHandler : IRequestHandler<CheckEmailQuery, bool>
    {
        private readonly IUserRepository repository;
        public CheckEmailQueryHandler(IUserRepository patientRepository)
        {
            this.repository = patientRepository;
        }
        public async Task<bool> Handle(CheckEmailQuery request, CancellationToken cancellationToken)
        {
            return await repository.ExistsByEmailAsync(request.Email);
        }
    }
}
