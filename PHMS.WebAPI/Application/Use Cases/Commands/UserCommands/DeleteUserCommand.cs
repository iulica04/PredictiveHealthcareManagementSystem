using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.UserCommands
{
    public class DeleteUserCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }
}
