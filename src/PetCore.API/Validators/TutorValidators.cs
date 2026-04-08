using FluentValidation;
using PetCore.API.DTOs.Tutors;

namespace PetCore.API.Validators;

public class CreateTutorRequestValidator : AbstractValidator<CreateTutorRequest>
{
    public CreateTutorRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cpf).MaximumLength(14);
        RuleFor(x => x.Phone).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.State).MaximumLength(2);
        RuleFor(x => x.ZipCode).MaximumLength(10);
    }
}
