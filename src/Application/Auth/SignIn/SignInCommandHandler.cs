using Application.Abstractions;
using Application.Abstractions.Tokens;
using Domain.Primitives;
using Mediator;

namespace Application.Auth.SignIn;

public sealed class SignInCommandHandler(
    IIdentityService identityService,
    ITokenGenerator tokenGenerator)
    : ICommandHandler<SignInCommand, Result<LoginResponse>>
{
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromHours(1);

    public async ValueTask<Result<LoginResponse>> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.ValidateCredentialsAsync(
            command.Email,
            command.Password,
            cancellationToken);

        if (result.IsFailure)
            return result.Error;

        var (userId, roles) = result.Value;
        var accessToken = tokenGenerator.GenerateAccessToken(userId, roles, AccessTokenLifetime);

        return new LoginResponse(accessToken);
    }
}
