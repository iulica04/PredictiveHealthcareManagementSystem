using Application.DTOs;
using Application.Queries;
using Application.Utils;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Gridify;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.UserQueryHandlers
{
    public class GetFilteredUsersQueryHandler : IRequestHandler<GetFilteredQuery<User, UserDto>, Result<PagedResult<UserDto>>>
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        public GetFilteredUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }
        public async Task<Result<PagedResult<UserDto>>> Handle(GetFilteredQuery<User, UserDto> request, CancellationToken cancellationToken)
        {
            var result = await userRepository.GetUsersOfTypeAsync(request.Type);
            if (!result.IsSuccess)
            {
                return Result<PagedResult<UserDto>>.Failure(result.ErrorMessage);
            }

            var users = result.Data!;
            if (request.Filter is not null)
            {
                users = users.AsQueryable().Where(request.Filter);
            }

            var totalCount = users.Count();
            var pagedUsers = users.AsQueryable().ApplyPaging(request.Page, request.PageSize);

            var userDtos = mapper.Map<IEnumerable<UserDto>>(pagedUsers);
            var pagedResult = new PagedResult<UserDto>(userDtos.ToList(), totalCount);

            return Result<PagedResult<UserDto>>.Success(pagedResult);
        }
    }
}
