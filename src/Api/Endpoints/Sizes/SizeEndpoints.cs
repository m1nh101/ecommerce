using Application.Sizes;
using Domain.Products;

namespace Api.Endpoints.Sizes;

internal static class SizeEndpoints
{
    internal static IEndpointRouteBuilder MapSizeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sizes", async (
            string? sizeType,
            ISizeService sizeService,
            CancellationToken cancellationToken) =>
        {
            var sizes = await sizeService.GetAllAsync(sizeType, cancellationToken);
            return Results.Ok(sizes);
        })
        .WithName("GetSizes")
        .WithTags("Sizes")
        .WithSummary("List all sizes, optionally filtered by type")
        .AllowAnonymous()
        .Produces<IReadOnlyList<SizeResponse>>(StatusCodes.Status200OK);

        app.MapPost("/api/v1/sizes", async (
            AddSizeRequest request,
            ISizeService sizeService,
            CancellationToken cancellationToken) =>
        {
            var result = await sizeService.AddAsync(request, cancellationToken);

            if (result.IsSuccess)
                return Results.Created($"/api/v1/sizes/{result.Value}", new { SizeId = result.Value });

            return result.Error.Code switch
            {
                "Size.DuplicateNameAndType" => Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflict",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("AddSize")
        .WithTags("Sizes")
        .WithSummary("Create a new size")
        .RequireAuthorization()
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        app.MapPatch("/api/v1/sizes/{sizeId:guid}", async (
            Guid sizeId,
            UpdateSizeRequest request,
            ISizeService sizeService,
            CancellationToken cancellationToken) =>
        {
            var result = await sizeService.UpdateAsync(new SizeId(sizeId), request, cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Size.NotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("UpdateSize")
        .WithTags("Sizes")
        .WithSummary("Update a size's sort order")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        app.MapDelete("/api/v1/sizes/{sizeId:guid}", async (
            Guid sizeId,
            ISizeService sizeService,
            CancellationToken cancellationToken) =>
        {
            var result = await sizeService.DeleteAsync(new SizeId(sizeId), cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Size.NotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error")
            };
        })
        .WithName("DeleteSize")
        .WithTags("Sizes")
        .WithSummary("Delete a size")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
