using FluentValidation;

namespace Application.Commands.Medic
{
    public class DeleteMedicCommandByIdValidator : AbstractValidator<DeleteMedicCommandById>
    {
        public DeleteMedicCommandByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
