using FluentValidation;
using Roboto.Dtos;

namespace Roboto.Service.Validators
{
    public class CalendarEventCreateDtoValidator : AbstractValidator<CalendarEventCreateDto>
    {
        public CalendarEventCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Event title is required")
                .Length(1, 200).WithMessage("Title must be between 1 and 200 characters");

            RuleFor(x => x.StartDateTime)
                .NotEmpty().WithMessage("Start date and time is required")
                .GreaterThan(DateTime.MinValue).WithMessage("Invalid start date/time");

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration must be greater than 0")
                .LessThanOrEqualTo(1440).WithMessage("Duration cannot exceed 24 hours (1440 hours)")
                .Must(d => d >= 0.25).WithMessage("Minimum duration is 15 minutes (0.25 hours)");

            RuleFor(x => x.Details)
                .MaximumLength(1000).WithMessage("Details cannot exceed 1000 characters");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Valid User ID is required");
        }
    }
}
