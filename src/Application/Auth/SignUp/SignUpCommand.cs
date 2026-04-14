using Domain.Primitives;
using Mediator;

namespace Application.Auth.SignUp;

public record SignUpCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName) : ICommand<Result<Guid>>;
