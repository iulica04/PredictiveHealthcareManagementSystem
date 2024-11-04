using FluentValidation;

namespace Application.Commands
{
    public class DeleteMedicCommandByIdValidator : AbstractValidator<DeleteMedicCommandById>
    {
        public DeleteMedicCommandByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
