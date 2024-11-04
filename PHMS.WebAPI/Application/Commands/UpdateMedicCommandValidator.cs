using FluentValidation;

namespace Application.Commands
{
    public class UpdateMedicCommandValidator : AbstractValidator<UpdateMedicCommand>
    {
        public UpdateMedicCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().Must(BeAValidGuid).WithMessage("Must be a valid guid");
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.Rank).NotEmpty();
            RuleFor(x => x.Specialization).NotEmpty();
            RuleFor(x => x.Hospital).NotEmpty();
        }

        private bool BeAValidGuid(Guid guid)
        {
            return Guid.TryParse(guid.ToString(), out _);
        }
    }
}
