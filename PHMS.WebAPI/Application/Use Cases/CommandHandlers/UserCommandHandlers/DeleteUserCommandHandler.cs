using Application.Use_Cases.Commands.UserCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.UserCommandHandlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<Unit>>
    {
        private readonly IUserRepository userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return Result<Unit>.Failure("Invalid user ID");
            }

            var userResult = await userRepository.GetUserByIdAsync(request.Id);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                return Result<Unit>.Failure("User not found");
            }
            await userRepository.DeleteUserAsync(request.Id);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
