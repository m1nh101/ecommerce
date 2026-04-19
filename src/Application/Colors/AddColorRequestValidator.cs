using FluentValidation;

namespace Application.Colors;

public sealed class AddColorRequestValidator : AbstractValidator<AddColorRequest>
{
    public AddColorRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.HexCode).NotEmpty().Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("HexCode must be in #RRGGBB format.");
    }
}
