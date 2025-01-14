using FluentValidation;

namespace Application.Use_Cases.Commands.TreatmentCommands
{
    public abstract class TreatmentCommandValidator<T, U>: AbstractValidator<T> where T : TreatmentCommand<U>
    {
        protected TreatmentCommandValidator()
        {
            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(100).WithMessage("Location must not exceed 100 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("StartDate cannot be in the future.");

            RuleFor(x => x.Duration)
                .NotEmpty().WithMessage("Duration is required.")
                .GreaterThan(x => x.StartDate).WithMessage("Duration must be after StartDate.");

            RuleFor(x => x.Frequency)
                .NotEmpty().WithMessage("Frequency is required.")
                .MaximumLength(50).WithMessage("Frequency must not exceed 50 characters.");
        }

        protected static bool BeAValidGuid(Guid guid)
        {
            return Guid.TryParse(guid.ToString(), out _);
        }
    }
}
