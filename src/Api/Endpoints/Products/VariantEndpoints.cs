using Application.Products.Variants.Add;
using Application.Products.Variants.Remove;
using Application.Products.Variants.Update;
using Mediator;

namespace Api.Endpoints.Products;

internal static class VariantEndpoints
{
    internal static IEndpointRouteBuilder MapVariantEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/products/{productId:guid}/variants", async (
            Guid productId,
            AddVariantRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new AddVariantCommand(
                productId,
                request.ColorId,
                request.SizeId,
                request.Sku,
                request.PriceOverride,
                request.PriceOverrideCurrency,
                request.StockQuantity);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.Created($"/api/v1/products/{productId}/variants/{result.Value}", new { VariantId = result.Value });

            return result.Error.Code switch
            {
                "Product.NotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                "Product.DuplicateVariant" or "Product.DuplicateSku" => Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflict",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("AddVariant")
        .WithTags("Product Variants")
        .WithSummary("Add a variant to a product")
        .RequireAuthorization()
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        app.MapPatch("/api/v1/products/{productId:guid}/variants/{variantId:guid}", async (
            Guid productId,
            Guid variantId,
            UpdateVariantRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateVariantCommand(
                productId,
                variantId,
                request.PriceOverride,
                request.PriceOverrideCurrency,
                request.StockQuantity,
                request.IsActive);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Product.NotFound" or "Product.VariantNotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("UpdateVariant")
        .WithTags("Product Variants")
        .WithSummary("Update a product variant")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        app.MapDelete("/api/v1/products/{productId:guid}/variants/{variantId:guid}", async (
            Guid productId,
            Guid variantId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RemoveVariantCommand(productId, variantId), cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Product.VariantNotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error")
            };
        })
        .WithName("RemoveVariant")
        .WithTags("Product Variants")
        .WithSummary("Remove a variant from a product")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}

internal record AddVariantRequest(
    Guid ColorId,
    Guid SizeId,
    string Sku,
    decimal? PriceOverride,
    string? PriceOverrideCurrency,
    int StockQuantity);

internal record UpdateVariantRequest(
    decimal? PriceOverride,
    string? PriceOverrideCurrency,
    int StockQuantity,
    bool IsActive);
