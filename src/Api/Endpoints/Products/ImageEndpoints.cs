using Application.Products.Images.Add;
using Application.Products.Images.Remove;
using Application.Products.Images.Update;
using Mediator;

namespace Api.Endpoints.Products;

internal static class ImageEndpoints
{
    internal static IEndpointRouteBuilder MapImageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/products/{productId:guid}/images", async (
            Guid productId,
            AddImageRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new AddImageCommand(
                productId,
                request.Url,
                request.VariantId,
                request.IsPrimary,
                request.SortOrder);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.Created($"/api/v1/products/{productId}/images/{result.Value}", new { ImageId = result.Value });

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
        .WithName("AddImage")
        .WithTags("Product Images")
        .WithSummary("Add an image to a product")
        .RequireAuthorization()
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        app.MapPatch("/api/v1/products/{productId:guid}/images/{imageId:guid}", async (
            Guid productId,
            Guid imageId,
            UpdateImageRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateImageCommand(productId, imageId, request.IsPrimary);
            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Product.NotFound" or "Product.ImageNotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("UpdateImage")
        .WithTags("Product Images")
        .WithSummary("Update a product image (e.g. set as primary)")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        app.MapDelete("/api/v1/products/{productId:guid}/images/{imageId:guid}", async (
            Guid productId,
            Guid imageId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RemoveImageCommand(productId, imageId), cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Product.NotFound" or "Product.ImageNotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error")
            };
        })
        .WithName("RemoveImage")
        .WithTags("Product Images")
        .WithSummary("Remove an image from a product")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}

internal record AddImageRequest(string Url, Guid? VariantId, bool IsPrimary, int SortOrder);
internal record UpdateImageRequest(bool IsPrimary);
