using Application.Commands.Patient;
<<<<<<< HEAD
=======
using Application.Utils;
>>>>>>> origin/iulia
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
<<<<<<< HEAD
using Domain.Services;
=======
>>>>>>> origin/iulia
using MediatR;

namespace Application.CommandHandlers.PatientCommandHandlers
{
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<Guid>>
    {
        private readonly IPatientRepository repository;
        private readonly IMapper mapper;

        public CreatePatientCommandHandler(IPatientRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
<<<<<<< HEAD
            var patient = mapper.Map<Patient>(request);
            patient.PasswordHash = PasswordHasher.HashPassword(request.Password);
            var result = await repository.AddAsync(patient);
=======
            var patien = mapper.Map<Patient>(request);
            patien.PasswordHash = PasswordHasher.HashPassword(request.Password);
            var result = await repository.AddAsync(patien);
>>>>>>> origin/iulia
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
