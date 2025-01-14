using MediatR;

namespace Application.Use_Cases.Commands.ConsultationCommands
{
    public class UpdateConsultationCommand : ConsultationCommand<Unit>
    {
        public Guid Id { get; set; }
    }
}
