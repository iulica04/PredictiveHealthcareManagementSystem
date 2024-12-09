﻿using Application.Commands.Administrator;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;


namespace Application.CommandHandlers.AdminCommandHandlers
{
    public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand, Result<Unit>>
    {
        private readonly IAdminRepository adminRepository;
        private readonly IMapper mapper;

        public UpdateAdminCommandHandler(IAdminRepository adminRepository, IMapper mapper)
        {
            this.adminRepository = adminRepository;
            this.mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
        {
            var admin = await adminRepository.GetByIdAsync(request.Id);
            if (admin == null)
            {
                return Result<Unit>.Failure("Admin not found");
            }

            admin = mapper.Map(request, admin);
            await adminRepository.UpdateAsync(admin);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
