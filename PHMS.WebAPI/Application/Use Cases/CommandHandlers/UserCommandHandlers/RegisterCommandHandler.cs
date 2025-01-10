using Application.Use_Cases.Commands.UserCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
    {
        private readonly IUserRepository UserRepository;
        private readonly IMapper Mapper;

        public RegisterCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            UserRepository = userRepository;
            Mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var emailExists = await UserRepository.ExistsByEmailAsync(request.Email);
            if (emailExists)
            {
                return Result<Guid>.Failure("Email already exists");
            }

            User newUser = request.Type == UserType.Medic ? Mapper.Map<Medic>(request) : Mapper.Map<Patient>(request);
            newUser.PasswordHash = PasswordHasher.HashPassword(request.Password);

            var result = await UserRepository.Register(newUser, cancellationToken);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(newUser.Id);
            }

            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
