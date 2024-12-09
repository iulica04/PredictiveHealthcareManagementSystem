using FluentValidation;
using Application.Commands.TreatmentCommands;


namespace Application.Commands.TreatmentCommands
{
    public class TreatmentValidator : AbstractValidator<CreateTreatmentCommand>
    {
        public TreatmentValidator()
        {

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.")
                .Must(type => type == "Drug" || type == "NaturalRemedy"|| type == "Surgery" || type == "Admission")
                .WithMessage("Type must be one of the following: 'Drug', 'NaturalRemedy', 'Surgery', 'Admission'.");

            RuleFor(x => x.Prescription)
                .NotNull().WithMessage("Prescription is required.");

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
    }
}