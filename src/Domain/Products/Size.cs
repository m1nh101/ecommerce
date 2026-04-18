using Domain.Enums;
using Domain.Primitives;

namespace Domain.Products;

public sealed class Size : AggregateRoot<SizeId>
{
    private Size() { }

    private Size(SizeId id, string name, SizeType sizeType, int sortOrder) : base(id)
    {
        Name = name;
        SizeType = sizeType;
        SortOrder = sortOrder;
    }

    public string Name { get; private set; } = null!;
    public SizeType SizeType { get; private set; }
    public int SortOrder { get; private set; }

    public static Result<Size> Create(string name, SizeType sizeType, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Size>(SizeErrors.InvalidName);

        return new Size(SizeId.New(), name, sizeType, sortOrder);
    }

    public void UpdateSortOrder(int sortOrder) => SortOrder = sortOrder;
}
