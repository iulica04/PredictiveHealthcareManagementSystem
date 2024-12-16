using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IUserRepository UserRepository;
        private readonly IMapper Mapper;

        public RegisterUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            UserRepository = userRepository;
            Mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = Mapper.Map<User>(request);
            return await UserRepository.Register(user, cancellationToken);
        }
    }
}
