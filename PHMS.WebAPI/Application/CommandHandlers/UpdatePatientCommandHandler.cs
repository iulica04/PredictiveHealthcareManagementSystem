
using Application.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand>
    {
        private readonly IPatientRepository patientRepository;
        private readonly IMapper mapper;

        public UpdatePatientCommandHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            this.patientRepository = patientRepository;
            this.mapper = mapper;
        }
        public Task Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = mapper.Map<Patient>(request);
            patient.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            return patientRepository.UpdateAsync(patient);
        }
    }
}
