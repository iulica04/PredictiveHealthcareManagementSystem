using FluentValidation;

namespace Application.Commands
{
    public class DeletePatientCommandByIdValidator : AbstractValidator<DeletePatientByIdCommand>
    {
        public DeletePatientCommandByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
