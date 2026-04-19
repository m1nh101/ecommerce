using FluentValidation;

namespace Application.Colors;

public sealed class UpdateColorRequestValidator : AbstractValidator<UpdateColorRequest>
{
    public UpdateColorRequestValidator()
    {
        RuleFor(x => x.HexCode).NotEmpty().Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("HexCode must be in #RRGGBB format.");
    }
}
