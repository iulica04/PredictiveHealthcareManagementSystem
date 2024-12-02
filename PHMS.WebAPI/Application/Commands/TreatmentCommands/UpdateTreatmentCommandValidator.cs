using Application.Commands.TreatmentCommands;
using Domain.Enums;
using FluentValidation;

namespace Application.Commands.TreatmentCommands
{
    public class UpdateTreatmentCommandValidator : AbstractValidator<UpdateTreatmentCommand>
    {
        public UpdateTreatmentCommandValidator()
        {
            RuleFor(x => x.TreatmentId)
                .NotEmpty().WithMessage("Id is required.")
                .Must(BeAValidGuid).WithMessage("Invalid Id format.");
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.")
                .Must(type => type == TreatmentType.Drug || type == TreatmentType.NaturalRemedy || type == TreatmentType.Surgery || type == TreatmentType.Admission)
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
        private static bool BeAValidGuid(Guid guid)
        {
            return Guid.TryParse(guid.ToString(), out _);
        }
    }
}
