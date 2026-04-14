using Application.Products.Add;
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
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        return app;
    }
}
