using Application.Use_Cases.Commands.UserCommands;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.UserCommandHandlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Unit>>
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }
        public async Task<Result<Unit>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await userRepository.GetUserByIdAsync(request.Id);
            if (!existingUser.IsSuccess)
            {
                return Result<Unit>.Failure(existingUser.ErrorMessage);
            }
            var foundUser = existingUser.Data!;

            if (!string.IsNullOrEmpty(request.Password))
            {
                foundUser.PasswordHash = PasswordHasher.HashPassword(request.Password);
            }
            mapper.Map(request, foundUser);
            await userRepository.UpdateUserAsync(foundUser);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
