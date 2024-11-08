using FluentValidation;

namespace Application.Commands.Patient
{
    public class DeletePatientCommandByIdValidator : AbstractValidator<DeletePatientByIdCommand>
    {
        public DeletePatientCommandByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
