using Application.Common;
using Application.Products.Add;
using Application.Products.Delete;
using Application.Products.Detail;
using Application.Products.Get;
using Application.Products.Update;
using Mediator;

namespace Api.Endpoints.Products;

internal static class ProductEndpoints
{
    internal static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/products", async (
            [AsParameters] GetProductsQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error");
        })
        .WithName("GetProducts")
        .WithTags("Products")
        .WithSummary("Get paginated list of products")
        .AllowAnonymous()
        .Produces<PagedResponse<ProductListItemResponse>>(StatusCodes.Status200OK)
        .ProducesValidationProblem();

        app.MapGet("/api/v1/products/{productId:guid}", async (
            Guid productId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetProductQuery(productId), cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(result.Value);

            return result.Error.Code switch
            {
                "Product.NotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error")
            };
        })
        .WithName("GetProduct")
        .WithTags("Products")
        .WithSummary("Get product by ID")
        .RequireAuthorization()
        .Produces<ProductDetailResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost("/api/v1/products", async (
            AddProductCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.Created($"/api/v1/products/{result.Value}", new { ProductId = result.Value });

            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                detail: result.Error.Description);
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
                request.Brand,
                request.CategoryId,
                request.Gender,
                request.BasePrice,
                request.Currency,
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

        app.MapDelete("/api/v1/products/{productId:guid}", async (
            Guid productId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteProductCommand(productId), cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Product.NotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error")
            };
        })
        .WithName("DeleteProduct")
        .WithTags("Products")
        .WithSummary("Soft delete a product (sets isActive = false)")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
