using Application.Auth.SignIn;
using Application.Auth.SignUp;
using Domain.Users;
using Mediator;

namespace Api.Endpoints.Auth;

internal static class AuthEndpoints
{
    internal static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/sign-in", async (
            SignInCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(result.Value);

            return result.Error.Code switch
            {
                "Auth.AccountLockedOut" => Results.Problem(
                    statusCode: StatusCodes.Status423Locked,
                    title: "Account locked",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Unauthorized",
                    detail: result.Error.Description)
            };
        })
        .WithName("SignIn")
        .WithTags("Auth")
        .WithSummary("Sign in with email and password")
        .Produces<LoginResponse>()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status423Locked)
        .ProducesValidationProblem();

        app.MapPost("/api/v1/sign-up", async (
            SignUpCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.Created($"/api/v1/users/{result.Value}", new { UserId = result.Value });

            return result.Error.Code switch
            {
                "User.EmailAlreadyInUse" => Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflict",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("SignUp")
        .WithTags("Auth")
        .WithSummary("Register a new user account")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        return app;
    }
}
