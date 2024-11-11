using Domain.Common;
using MediatR;

namespace Application.Commands.Administrator
{
    public class DeleteAdminByIdCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }

}
