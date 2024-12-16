using Application.DTOs;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Use_Cases.Queries.UserQueries
{
    public class GetUsersOfTypeQuery: IRequest<Result<IEnumerable<UserDto>>>
    {
        public UserType Type { get; set; }
    }
}
