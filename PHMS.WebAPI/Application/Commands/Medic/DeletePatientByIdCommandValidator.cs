using FluentValidation;

namespace Application.Commands.Medic
{
    public class DeletePatientByIdCommandValidator : AbstractValidator<DeletePatientByIdCommand>
    {
        public DeletePatientByIdCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
