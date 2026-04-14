using Application.Abstractions;
using Domain.Primitives;
using Domain.Users;
using Mediator;

namespace Application.Auth.SignUp;

public sealed class SignUpCommandHandler(
    IIdentityService identityService,
    IApplicationDbContext dbContext)
    : ICommandHandler<SignUpCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        var registerResult = await identityService.RegisterAsync(
            command.Email,
            command.Password,
            cancellationToken);

        if (registerResult.IsFailure)
            return registerResult.Error;

        var user = User.Create(registerResult.Value, command.Email, command.FirstName, command.LastName);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Id.Value;
    }
}
