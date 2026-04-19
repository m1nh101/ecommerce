using FluentValidation;

namespace Application.Sizes;

public sealed class UpdateSizeRequestValidator : AbstractValidator<UpdateSizeRequest>
{
    public UpdateSizeRequestValidator()
    {
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
