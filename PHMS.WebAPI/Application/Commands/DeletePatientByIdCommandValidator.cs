using Application.Commands.Patient;
using FluentValidation;

namespace Application.Commands
{
    public class DeletePatientByIdCommandValidator : AbstractValidator<DeletePatientByIdCommand>
    {
        public DeletePatientByIdCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
