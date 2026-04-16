using Application.Carts.AddItem;
using Application.Carts.EmptyCart;
using Application.Carts.Get;
using Application.Carts.RemoveItem;
using Application.Carts.UpdateItemQuantity;
using Mediator;

namespace Api.Endpoints.Carts;

internal static class CartEndpoints
{
    internal static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/cart", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetCartQuery(), cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error");
        })
        .WithName("GetCart")
        .WithTags("Cart")
        .WithSummary("Get the current user's cart")
        .RequireAuthorization()
        .Produces<CartResponse>(StatusCodes.Status200OK);

        app.MapPost("/api/v1/cart/items", async (
            AddCartItemRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new AddCartItemCommand(
                request.ProductId,
                request.ProductName,
                request.UnitPrice,
                request.Quantity);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Cart.InvalidQuantity" => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("AddCartItem")
        .WithTags("Cart")
        .WithSummary("Add an item to the cart")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        app.MapPut("/api/v1/cart/items/{productId:guid}", async (
            Guid productId,
            UpdateCartItemQuantityRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCartItemQuantityCommand(productId, request.Quantity);
            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Cart.ItemNotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                "Cart.InvalidQuantity" => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("UpdateCartItemQuantity")
        .WithTags("Cart")
        .WithSummary("Update the quantity of a cart item")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        app.MapDelete("/api/v1/cart/items/{productId:guid}", async (
            Guid productId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveCartItemCommand(productId);
            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Cart.ItemNotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error")
            };
        })
        .WithName("RemoveCartItem")
        .WithTags("Cart")
        .WithSummary("Remove an item from the cart")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapDelete("/api/v1/cart", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            await mediator.Send(new EmptyCartCommand(), cancellationToken);
            return Results.NoContent();
        })
        .WithName("EmptyCart")
        .WithTags("Cart")
        .WithSummary("Empty the cart")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent);

        return app;
    }
}

internal record AddCartItemRequest(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity);

internal record UpdateCartItemQuantityRequest(int Quantity);
