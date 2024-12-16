using Application.Queries;
using Domain.Repositories;
using MediatR;

namespace Application.QueryHandlers
{
    public class CheckEmailQueryHandler : IRequestHandler<CheckEmailQuery, bool>
    {
        private readonly IUserRepository repository;
        public CheckEmailQueryHandler(IUserRepository userRepository)
        {
            this.repository = userRepository;
        }
        public async Task<bool> Handle(CheckEmailQuery request, CancellationToken cancellationToken)
        {
            return await repository.ExistsByEmailAsync(request.Email);
        }
    }
}
