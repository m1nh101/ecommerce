using Application.Colors;
using Domain.Products;

namespace Api.Endpoints.Colors;

internal static class ColorEndpoints
{
    internal static IEndpointRouteBuilder MapColorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/colors", async (
            IColorService colorService,
            CancellationToken cancellationToken) =>
        {
            var colors = await colorService.GetAllAsync(cancellationToken);
            return Results.Ok(colors);
        })
        .WithName("GetColors")
        .WithTags("Colors")
        .WithSummary("List all colors")
        .AllowAnonymous()
        .Produces<IReadOnlyList<ColorResponse>>(StatusCodes.Status200OK);

        app.MapPost("/api/v1/colors", async (
            AddColorRequest request,
            IColorService colorService,
            CancellationToken cancellationToken) =>
        {
            var result = await colorService.AddAsync(request, cancellationToken);

            if (result.IsSuccess)
                return Results.Created($"/api/v1/colors/{result.Value}", new { ColorId = result.Value });

            return result.Error.Code switch
            {
                "Color.DuplicateName" => Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflict",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("AddColor")
        .WithTags("Colors")
        .WithSummary("Create a new color")
        .RequireAuthorization()
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        app.MapPatch("/api/v1/colors/{colorId:guid}", async (
            Guid colorId,
            UpdateColorRequest request,
            IColorService colorService,
            CancellationToken cancellationToken) =>
        {
            var result = await colorService.UpdateAsync(new ColorId(colorId), request, cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Color.NotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("UpdateColor")
        .WithTags("Colors")
        .WithSummary("Update a color's hex code")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        app.MapDelete("/api/v1/colors/{colorId:guid}", async (
            Guid colorId,
            IColorService colorService,
            CancellationToken cancellationToken) =>
        {
            var result = await colorService.DeleteAsync(new ColorId(colorId), cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Color.NotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error")
            };
        })
        .WithName("DeleteColor")
        .WithTags("Colors")
        .WithSummary("Delete a color")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
