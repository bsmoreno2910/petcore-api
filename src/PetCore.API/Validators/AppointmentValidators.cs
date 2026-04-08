using FluentValidation;
using PetCore.API.DTOs.Appointments;

namespace PetCore.API.Validators;

public class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.ScheduledAt).NotEmpty().GreaterThan(DateTime.UtcNow.AddMinutes(-5));
        RuleFor(x => x.DurationMinutes).InclusiveBetween(5, 480);
    }
}
