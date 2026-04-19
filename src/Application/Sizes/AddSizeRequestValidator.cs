using Domain.Enums;
using FluentValidation;

namespace Application.Sizes;

public sealed class AddSizeRequestValidator : AbstractValidator<AddSizeRequest>
{
    public AddSizeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(20);
        RuleFor(x => x.SizeType)
            .NotEmpty()
            .Must(t => Enum.TryParse<SizeType>(t, ignoreCase: true, out _))
            .WithMessage("SizeType must be one of: Clothing, Shoe, Accessory.");
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
