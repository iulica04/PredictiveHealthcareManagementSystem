
using FluentValidation;

namespace Application.Commands
{
    public class UpdateMedicCommandValidator : AbstractValidator<UpdateMedicCommand>
    {
        public UpdateMedicCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.")
                .Must(BeAValidGuid).WithMessage("Invalid Id format.");

            RuleFor(x => x.FirstName)
                .NotNull().WithMessage("First name is required.")
                .NotEmpty().WithMessage("First name cannot be empty.")
                .MaximumLength(30).WithMessage("First name must be at most 30 characters.");

            RuleFor(x => x.LastName)
                .NotNull().WithMessage("Last name is required.")
                .NotEmpty().WithMessage("Last name cannot be empty.")
                .MaximumLength(30).WithMessage("Last name must be at most 30 characters.");

            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");


            RuleFor(x => x.PhoneNumber)
                .NotNull().WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Password)
               .NotNull().WithMessage("Password is required.")
               .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
               .MaximumLength(100).WithMessage("Password must be at most 100 characters long.")
               .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
               .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
               .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
               .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.Gender)
                .NotNull().WithMessage("Gender is required.")
                .Must(g => g == "Male" || g == "Female")
                .WithMessage("Gender must be either 'Male' or 'Female'.");

            RuleFor(x => x.BirthDate)
                .NotNull().WithMessage("Birthday is required.")
                .LessThan(DateTime.Now).WithMessage("Birthday must be in the past.");

            RuleFor(x => x.Address)
                .NotNull().WithMessage("Address is required.");
            RuleFor(x => x.Rank)
                .NotNull().WithMessage("Rank is required.")
                .MaximumLength(30).WithMessage("Rank must be at most 30 characters.");
            RuleFor(x => x.Specialization)
                .NotNull().WithMessage("Specialization is required.")
                .MaximumLength(30).WithMessage("Specialization must be at most 30 characters.");
            RuleFor(x => x.Hospital)
                .NotNull().WithMessage("Hospital is required.")
                .MaximumLength(30).WithMessage("Hospital must be at most 30 characters.");

        }
        private bool BeAValidGuid(Guid guid)
        {
            return Guid.TryParse(guid.ToString(), out _);
        }
    }
}