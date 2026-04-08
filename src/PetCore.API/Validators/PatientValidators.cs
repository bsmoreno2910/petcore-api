using FluentValidation;
using PetCore.API.DTOs.Patients;

namespace PetCore.API.Validators;

public class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TutorId).NotEmpty();
        RuleFor(x => x.SpeciesId).NotEmpty();
        RuleFor(x => x.Weight).GreaterThan(0).When(x => x.Weight.HasValue);
    }
}
