using Domain.Primitives;

namespace Domain.Products;

public sealed class Category : AggregateRoot<CategoryId>
{
    private Category() { }

    private Category(CategoryId id, string name, string slug, string? description, int displayOrder)
        : base(id)
    {
        Name = name;
        Slug = slug;
        Description = description;
        DisplayOrder = displayOrder;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Result<Category> Create(string name, string slug, string? description, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Category>(CategoryErrors.InvalidName);

        if (string.IsNullOrWhiteSpace(slug))
            return Result.Failure<Category>(CategoryErrors.InvalidSlug);

        return new Category(CategoryId.New(), name, slug, description, displayOrder);
    }

    public void UpdateDetails(string name, string slug, string? description, int displayOrder)
    {
        Name = name;
        Slug = slug;
        Description = description;
        DisplayOrder = displayOrder;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
