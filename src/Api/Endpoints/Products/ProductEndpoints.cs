using Application.Products.Add;
using Application.Products.Update;
using Mediator;

namespace Api.Endpoints.Products;

internal static class ProductEndpoints
{
    internal static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/products", async (
            AddProductCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.Created($"/api/v1/products/{result.Value}", new { ProductId = result.Value });

            return result.Error.Code switch
            {
                "Product.InvalidPrice" => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description),
                "Product.InvalidStockQuantity" => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("AddProduct")
        .WithTags("Products")
        .WithSummary("Create a new product")
        .RequireAuthorization()
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        app.MapPatch("/api/v1/products/{productId:guid}", async (
            Guid productId,
            UpdateProductRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateProductCommand(
                productId,
                request.Name,
                request.Description,
                request.Price,
                request.Currency,
                request.StockQuantity,
                request.Category,
                request.IsActive);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Product.NotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                "Product.InvalidPrice" or
                "Product.InvalidStockQuantity" or
                "Product.InsufficientStock" => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("UpdateProduct")
        .WithTags("Products")
        .WithSummary("Update an existing product")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        return app;
    }
}
