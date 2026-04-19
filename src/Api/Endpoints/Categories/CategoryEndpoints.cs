using Application.Categories;
using Domain.Products;

namespace Api.Endpoints.Categories;

internal static class CategoryEndpoints
{
    internal static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/categories", async (
            ICategoryService categoryService,
            CancellationToken cancellationToken) =>
        {
            var categories = await categoryService.GetAllAsync(cancellationToken);
            return Results.Ok(categories);
        })
        .WithName("GetCategories")
        .WithTags("Categories")
        .WithSummary("List all categories")
        .AllowAnonymous()
        .Produces<IReadOnlyList<CategoryResponse>>(StatusCodes.Status200OK);

        app.MapPost("/api/v1/categories", async (
            AddCategoryRequest request,
            ICategoryService categoryService,
            CancellationToken cancellationToken) =>
        {
            var result = await categoryService.AddAsync(request, cancellationToken);

            if (result.IsSuccess)
                return Results.Created($"/api/v1/categories/{result.Value}", new { CategoryId = result.Value });

            return result.Error.Code switch
            {
                "Category.DuplicateSlug" => Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflict",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("AddCategory")
        .WithTags("Categories")
        .WithSummary("Create a new category")
        .RequireAuthorization()
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesValidationProblem();

        app.MapPatch("/api/v1/categories/{categoryId:guid}", async (
            Guid categoryId,
            UpdateCategoryRequest request,
            ICategoryService categoryService,
            CancellationToken cancellationToken) =>
        {
            var result = await categoryService.UpdateAsync(new CategoryId(categoryId), request, cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Category.NotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                "Category.DuplicateSlug" => Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflict",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad Request",
                    detail: result.Error.Description)
            };
        })
        .WithName("UpdateCategory")
        .WithTags("Categories")
        .WithSummary("Update a category")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesValidationProblem();

        app.MapDelete("/api/v1/categories/{categoryId:guid}", async (
            Guid categoryId,
            ICategoryService categoryService,
            CancellationToken cancellationToken) =>
        {
            var result = await categoryService.DeactivateAsync(new CategoryId(categoryId), cancellationToken);

            if (result.IsSuccess)
                return Results.NoContent();

            return result.Error.Code switch
            {
                "Category.NotFound" => Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not Found",
                    detail: result.Error.Description),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error")
            };
        })
        .WithName("DeleteCategory")
        .WithTags("Categories")
        .WithSummary("Deactivate a category")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
