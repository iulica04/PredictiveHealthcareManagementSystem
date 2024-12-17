﻿using Application.DTOs;
using Application.Use_Cases.Queries.UserQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.UserQueryHandlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return Result<UserDto>.Failure("Invalid user ID");
            }

            var user = await userRepository.GetUserByIdAsync(request.Id);
            if (user.IsSuccess && user.Data != null)
            {
                return Result<UserDto>.Success(mapper.Map<UserDto>(user.Data));
            }
            return Result<UserDto>.Failure(user.ErrorMessage ?? "User not found");
        }

    }
}