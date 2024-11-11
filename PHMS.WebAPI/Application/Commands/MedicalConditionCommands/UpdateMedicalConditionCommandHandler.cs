using FluentValidation;
using FluentValidation.Validators;

namespace Application.Commands.MedicalConditionCommands
{
    public class UpdateMedicalConditionCommandHandler : AbstractValidator<UpdateMedicalConditionCommand>
    {
        public UpdateMedicalConditionCommandHandler()
        {
            RuleFor(x => x.MedicalConditionId)
                 .NotEmpty().WithMessage("Id is required.")
                 .Must(BeAValidGuid).WithMessage("Invalid Id format.");
            RuleFor(x => x.PatientId)
              .NotEmpty().WithMessage("PatientId is required.").Must(BeAValidGuid).WithMessage("Invalid Id format."); ;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(1, 50).WithMessage("Name must be between 1 and 50 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("StartDate cannot be in the future.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("EndDate must be after StartDate.")
                .When(x => x.EndDate.HasValue);

            RuleFor(x => x.CurrentStatus)
                .NotEmpty().WithMessage("CurrentStatus is required.")
                .Must(x => x == "ongoing" || x == "cured" || x == "suspected" || x == "inactive")
                .WithMessage("CurrentStatus must be one of the following: 'ongoing', 'cured', 'suspected', 'inactive'.");

            RuleFor(x => x.IsGenetic)
                .NotNull().WithMessage("IsGenetic must be specified.").Must(x => x == true || x == false).WithMessage("IsGenetic must be a boolean value.");

            RuleFor(x => x.Recommendation)
                .NotEmpty().WithMessage("Recommendations are required.")
                .MaximumLength(500).WithMessage("Recommendations must not exceed 500 characters.");
        
    }
        private bool BeAValidGuid(Guid guid)
        {
            return Guid.TryParse(guid.ToString(), out _);
        }
    }
}
