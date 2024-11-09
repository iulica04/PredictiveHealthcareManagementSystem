using FluentValidation;

namespace Application.Commands
{
    public class DeleteMedicByIdCommandValidator : AbstractValidator<DeleteMedicByIdCommand>
    {
        public DeleteMedicByIdCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
