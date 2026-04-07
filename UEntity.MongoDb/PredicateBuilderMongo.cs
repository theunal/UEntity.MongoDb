using System.Linq.Expressions;

namespace UEntity.MongoDb;

public static class PredicateBuilderMongo
{
    public static Expression<Func<T, bool>> NewQuery<T>(bool @is) => x => @is;
    public static Expression<Func<T, bool>> NewQuery<T>(Expression<Func<T, bool>> predicate) => predicate;
    public static Expression<Func<T, bool>> AndMongo<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        if (first == null) return second;
        if (second == null) return first;

        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(
            ReplaceParameter(first.Body, first.Parameters[0], parameter),
            ReplaceParameter(second.Body, second.Parameters[0], parameter));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
    public static Expression<Func<T, bool>> OrMongo<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        if (first == null) return second;
        if (second == null) return first;
        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.OrElse(
            ReplaceParameter(first.Body, first.Parameters[0], parameter),
            ReplaceParameter(second.Body, second.Parameters[0], parameter));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
    private static Expression ReplaceParameter(Expression body, ParameterExpression oldParam, ParameterExpression newParam)
    {
        return new ParameterReplacer(oldParam, newParam).Visit(body);
    }
    private class ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam) : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParam = oldParam;
        private readonly ParameterExpression _newParam = newParam;
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParam ? _newParam : base.VisitParameter(node);
        }
    }

    public static PaginateMongo<TDestination> ConvertItems<TSource, TDestination>(this PaginateMongo<TSource> source, List<TDestination> items)
    {
        return new PaginateMongo<TDestination>
        {
            TotalCount = source.TotalCount,
            HasNext = source.HasNext,
            HasPrevious = source.HasPrevious,
            Page = source.Page,
            PagesCount = source.PagesCount,
            Size = source.Size,
            Items = items
        };
    }
}