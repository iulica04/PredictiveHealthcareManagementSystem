using ApplicationApplication.Use_Cases.Commands.ConsultationCommands;
using MediatR;

namespace Application.Use_Cases.Commands.ConsultationCommands
{
    public class UpdateConsultationCommandValidator : ConsultationCommandValidator<UpdateConsultationCommand, Unit>
    {
    }
}