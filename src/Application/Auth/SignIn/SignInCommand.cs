using Domain.Primitives;
using Mediator;

namespace Application.Auth.SignIn;

public record SignInCommand(string Email, string Password) : ICommand<Result<LoginResponse>>;

public record LoginResponse(string AccessToken, string TokenType = "Bearer");
