using FluentValidation;
using PetCore.API.DTOs.Clinics;

namespace PetCore.API.Validators;

public class CreateClinicRequestValidator : AbstractValidator<CreateClinicRequest>
{
    public CreateClinicRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cnpj).MaximumLength(18);
        RuleFor(x => x.Phone).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.State).MaximumLength(2);
        RuleFor(x => x.ZipCode).MaximumLength(10);
    }
}

public class AddClinicUserRequestValidator : AbstractValidator<AddClinicUserRequest>
{
    public AddClinicUserRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty();
    }
}
