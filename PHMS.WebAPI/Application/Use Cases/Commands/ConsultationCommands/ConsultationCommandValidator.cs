using Application.Use_Cases.Commands.ConsultationCommands;
using FluentValidation;

namespace ApplicationApplication.Use_Cases.Commands.ConsultationCommands
{
    public abstract class ConsultationCommandValidator<T, U> : AbstractValidator<T> where T : ConsultationCommand<U>
    {
        protected ConsultationCommandValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("PatientId is required.")
                .Must(BeAValidGuid).WithMessage("Invalid Id format.");

            RuleFor(x => x.MedicId)
                .NotEmpty().WithMessage("MedicId is required.")
                .Must(BeAValidGuid).WithMessage("Invalid Id format.");

            RuleFor(x => x.Date)
                .GreaterThan(DateTime.Now.AddMinutes(15)).WithMessage("Consultation date and time must be at least 15 minutes in the future.")
                .Must(BeWithinWorkingHours).WithMessage("Consultation time must be between 08:00 and 18:00.")
                .Must(NotBeOnWeekend).WithMessage("Consultation cannot be scheduled on a weekend.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(100).WithMessage("Location must not exceed 100 characters.");
        }

        protected static bool BeAValidGuid(Guid guid)
        {
            return Guid.TryParse(guid.ToString(), out _);
        }

        private static bool BeWithinWorkingHours(DateTime dateTime)
        {
            var startHour = new TimeSpan(8, 0, 0);  // 08:00
            var endHour = new TimeSpan(18, 0, 0);   // 18:00
            var consultationTime = dateTime.TimeOfDay;

            return consultationTime >= startHour && consultationTime <= endHour;
        }

        private static bool NotBeOnWeekend(DateTime dateTime)
        {
            return dateTime.DayOfWeek != DayOfWeek.Saturday && dateTime.DayOfWeek != DayOfWeek.Sunday;
        }
    }
}
