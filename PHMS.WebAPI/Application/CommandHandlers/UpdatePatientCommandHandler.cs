using Application.Commands.Patient;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.CommandHandlers
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Result<Unit>>
    {
        private readonly IPatientRepository patientRepository;
        private readonly IMapper mapper;

        public UpdatePatientCommandHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            this.patientRepository = patientRepository;
            this.mapper = mapper;
        }
        public async Task<Result<Unit>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await patientRepository.GetByIdAsync(request.Id);
            if (patient == null)
            {
                return Result<Unit>.Failure("Patient not found");
            }
            patient = mapper.Map(request, patient);
            await patientRepository.UpdateAsync(patient);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
