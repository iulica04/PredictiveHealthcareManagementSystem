
using Application.Use_Cases.Commands.ConsultationCommands;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ConsultationCommandHandler
{
    public class UpdateConsultationCommandHandler : IRequestHandler<UpdateConsultationCommand, Result<Unit>>
    {
        private readonly IConsultationRepository repository;
        private readonly IMapper mapper;

        public UpdateConsultationCommandHandler(IConsultationRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<Unit>> Handle(UpdateConsultationCommand request, CancellationToken cancellationToken)
        {
            var consultation = await repository.GetByIdAsync(request.Id);
            if (consultation == null)
            {
                return Result<Unit>.Failure("Consultation not found.");
            }

            mapper.Map(request, consultation);

            await repository.UpdateAsync(consultation);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
