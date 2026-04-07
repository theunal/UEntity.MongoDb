using System.Linq.Expressions;

namespace UEntity.MongoDb;

public record EntitySortModelMongo<T>
{
    public Expression<Func<T, object?>> Sort { get; set; } = null!;
    public bool IsDescending { get; set; }
}

public record PaginateMongo<T>
{
    public int Page { get; set; }
    public int Size { get; set; }
    public long TotalCount { get; set; }
    public long PagesCount { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
    public List<T> Items { get; set; } = null!;
}