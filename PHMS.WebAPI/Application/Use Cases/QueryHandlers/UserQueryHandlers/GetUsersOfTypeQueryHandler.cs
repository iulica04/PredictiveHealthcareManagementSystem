using Application.DTOs;
using Application.Use_Cases.Queries.UserQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.UserQueryHandlers
{
    public class GetUsersOfTypeQueryHandler : IRequestHandler<GetUsersOfTypeQuery, Result<IEnumerable<UserDto>>>
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public GetUsersOfTypeQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<Result<IEnumerable<UserDto>>> Handle(GetUsersOfTypeQuery request, CancellationToken cancellationToken)
        {
            var users = await userRepository.GetUsersOfTypeAsync(request.Type);
            if (users.IsSuccess)
            {
                return Result<IEnumerable<UserDto>>.Success(mapper.Map<IEnumerable<UserDto>>(users.Data));
            }
            return Result<IEnumerable<UserDto>>.Failure(users.ErrorMessage);
        }
    }
}
