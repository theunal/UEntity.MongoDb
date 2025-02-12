using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace UEntity.MongoDb;

/// <summary>
/// Repository interface for MongoDB operations on entities of type T.
/// </summary>
public interface IEntityRepositoryMongo<T> where T : IMongoEntity
{
    /// <summary>
    /// Retrieves a single document that matches the specified filter with optional sorting.
    /// </summary>
    /// <param name="filter">Filter expression for the query.</param>
    /// <param name="sort">Optional sort criteria.</param>
    /// <returns>The first matching document or null if not found.</returns>
    T? Get(Expression<Func<T, bool>> filter, EntitySortModelMongo<T>? sort = null);

    /// <summary>
    /// Asynchronously retrieves a single document that matches the specified filter with optional sorting.
    /// </summary>
    /// <param name="filter">Filter expression for the query.</param>
    /// <param name="sort">Optional sort criteria.</param>
    /// <returns>A task representing the asynchronous operation. The result contains the first matching document or null if not found.</returns>
    Task<T?> GetAsync(Expression<Func<T, bool>> filter, EntitySortModelMongo<T>? sort = null);

    /// <summary>
    /// Retrieves all documents that match the specified filter with optional sorting.
    /// </summary>
    /// <param name="filter">Filter expression for the query.</param>
    /// <param name="sort">Optional sort criteria.</param>
    /// <returns>A list of matching documents.</returns>
    List<T> GetAll(Expression<Func<T, bool>>? filter = null, EntitySortModelMongo<T>? sort = null);

    /// <summary>
    /// Asynchronously retrieves all documents that match the specified filter with optional sorting.
    /// </summary>
    /// <param name="filter">Filter expression for the query.</param>
    /// <param name="sort">Optional sort criteria.</param>
    /// <returns>A task representing the asynchronous operation. The result contains a list of matching documents.</returns>
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, EntitySortModelMongo<T>? sort = null);

    /// <summary>
    /// Retrieves paginated results based on the specified page, size, and optional filters and sorting.
    /// </summary>
    /// <param name="page">Page number (zero-based).</param>
    /// <param name="size">Number of items per page.</param>
    /// <param name="filter">Optional filter for the query.</param>
    /// <param name="sort">Optional sort criteria.</param>
    /// <returns>Paginated results.</returns>
    PaginateMongo<T> GetListPaginate(int page, int size, FilterDefinition<T>? filter = null, EntitySortModelMongo<T>? sort = null);

    /// <summary>
    /// Asynchronously retrieves paginated results based on the specified page, size, and optional filters and sorting.
    /// </summary>
    /// <param name="page">Page number (zero-based).</param>
    /// <param name="size">Number of items per page.</param>
    /// <param name="filter">Optional filter for the query.</param>
    /// <param name="sort">Optional sort criteria.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The result contains paginated results.</returns>
    Task<PaginateMongo<T>> GetListPaginateAsync(int page, int size, FilterDefinition<T>? filter = null, EntitySortModelMongo<T>? sort = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously inserts a single document into the collection.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts multiple documents into the collection.
    /// </summary>
    /// <param name="entities">The collection of entities to insert.</param>
    void AddRange(IEnumerable<T> entities);

    /// <summary>
    /// Asynchronously inserts multiple documents into the collection.
    /// </summary>
    /// <param name="entities">The collection of entities to insert.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a single document that matches the specified filter.
    /// </summary>
    /// <param name="filter">Filter expression for the document to update.</param>
    /// <param name="entity">The updated entity.</param>
    /// <returns>The result of the replace operation.</returns>
    ReplaceOneResult Update(Expression<Func<T, bool>> filter, T entity);

    /// <summary>
    /// Asynchronously updates a single document that matches the specified filter.
    /// </summary>
    /// <param name="filter">Filter expression for the document to update.</param>
    /// <param name="entity">The updated entity.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The result contains the replace operation result.</returns>
    Task<ReplaceOneResult> UpdateAsync(Expression<Func<T, bool>> filter, T entity, CancellationToken cancellationToken = default);
    Task<UpdateResult> ExecuteUpdateAsync(Expression<Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>>> updateExpression,
            Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a single document that matches the specified filter.
    /// </summary>
    /// <param name="filter">Filter expression for the document to delete.</param>
    /// <returns>The result of the delete operation.</returns>
    DeleteResult ExecuteDelete(Expression<Func<T, bool>> filter);

    /// <summary>
    /// Asynchronously deletes a single document that matches the specified filter.
    /// </summary>
    /// <param name="filter">Filter expression for the document to delete.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The result contains the delete operation result.</returns>
    Task<DeleteResult> ExecuteDeleteAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes a single document by deleting and re-inserting it.
    /// </summary>
    /// <param name="filter">Filter expression for the document to refresh.</param>
    /// <param name="entity">The updated entity.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    Task RefreshAsync(Expression<Func<T, bool>> filter, T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes multiple documents by deleting and re-inserting them.
    /// </summary>
    /// <param name="filter">Filter expression for the documents to refresh.</param>
    /// <param name="entities">The collection of updated entities.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    Task RefreshAllAsync(Expression<Func<T, bool>> filter, IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Selects all documents matching the filter and projects them into the specified result type.
    /// </summary>
    /// <typeparam name="TResult">The result type to project into.</typeparam>
    /// <param name="select">Projection expression.</param>
    /// <param name="filter">Optional filter expression.</param>
    /// <returns>A list of projected results.</returns>
    List<TResult> SelectAll<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null);
    Task<List<TResult>> SelectAllAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null);
    TResult Select<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null);
    Task<TResult> SelectAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null);

    /// <summary>
    /// Counts the number of documents matching the specified filter.
    /// </summary>
    /// <param name="filter">Optional filter expression.</param>
    /// <returns>The count of matching documents.</returns>
    int Count(Expression<Func<T, bool>>? filter = null);

    /// <summary>
    /// Asynchronously counts the number of documents matching the specified filter.
    /// </summary>
    /// <param name="filter">Optional filter expression.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The result contains the count of matching documents.</returns>
    Task<long> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether any documents match the specified filter.
    /// </summary>
    /// <param name="filter">Filter expression for the query.</param>
    /// <returns>True if any documents match the filter, otherwise false.</returns>
    bool Any(Expression<Func<T, bool>> filter);

    /// <summary>
    /// Asynchronously determines whether any documents match the specified filter.
    /// </summary>
    /// <param name="filter">Filter expression for the query.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The result contains true if any documents match the filter, otherwise false.</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the maximum value of the specified field.
    /// </summary>
    /// <typeparam name="TResult">The type of the field.</typeparam>
    /// <param name="filter">Field selector expression.</param>
    /// <returns>The maximum value or null if no documents are found.</returns>
    TResult? Max<TResult>(Expression<Func<T, TResult>> filter);

    /// <summary>
    /// Retrieves the minimum value of the specified field.
    /// </summary>
    /// <typeparam name="TResult">The type of the field.</typeparam>
    /// <param name="filter">Field selector expression.</param>
    /// <returns>The minimum value or null if no documents are found.</returns>
    TResult? Min<TResult>(Expression<Func<T, TResult>> filter);
}
public class EntityRepositoryMongo<T>(string databaseName) : IEntityRepositoryMongo<T> where T : IMongoEntity
{
    private readonly IMongoCollection<T> _collection = UEntityMongoDbExtension.UEntityMongoClient?.GetDatabase(databaseName)?.GetCollection<T>(typeof(T).Name) ??
        throw new ArgumentNullException("Please call the “AddUEntityMongoDb” method in your program record.");

    /* select */
    public T? Get(Expression<Func<T, bool>> filter, EntitySortModelMongo<T>? sort = null)
    {
        if (sort != null)
        {
            return _collection.Find(filter).Sort(GetSortDefinitionBuilder(sort)).Limit(1).FirstOrDefault();
        }
        return _collection.Find(filter).Limit(1).FirstOrDefault();
    }
    public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, EntitySortModelMongo<T>? sort = null)
    {
        if (sort != null)
        {
            return await _collection.Find(filter).Sort(GetSortDefinitionBuilder(sort)).Limit(1).FirstOrDefaultAsync();
        }
        return await _collection.Find(filter).Limit(1).FirstOrDefaultAsync();
    }
    public List<T> GetAll(Expression<Func<T, bool>>? filter = null, EntitySortModelMongo<T>? sort = null)
    {
        if (sort != null)
        {
            return _collection.Find(filter ?? FilterDefinition<T>.Empty).Sort(GetSortDefinitionBuilder(sort)).ToList();
        }
        return _collection.Find(filter ?? FilterDefinition<T>.Empty).ToList();
    }
    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, EntitySortModelMongo<T>? sort = null)
    {
        if (sort != null)
        {
            return await _collection.Find(filter ?? FilterDefinition<T>.Empty).Sort(GetSortDefinitionBuilder(sort)).ToListAsync();
        }
        return await _collection.Find(filter ?? FilterDefinition<T>.Empty).ToListAsync();
    }
    public PaginateMongo<T> GetListPaginate(int page, int size, FilterDefinition<T>? filter = null, EntitySortModelMongo<T>? sort = null)
    {
        filter ??= FilterDefinition<T>.Empty;
        var query = _collection.Find(filter);
        if (sort != null)
        {
            query = query.Sort(GetSortDefinitionBuilder(sort));
        }
        var count = (int)_collection.CountDocuments(filter);
        var items = query.Skip(page * size).Limit(size).ToList();
        return new PaginateMongo<T>
        {
            Index = page,
            Size = size,
            From = 1,
            Count = count,
            Items = items,
            Pages = (int)Math.Ceiling(count / (double)size),
            HasPrevious = page > 1,
            HasNext = page * size < count
        };
    }
    public async Task<PaginateMongo<T>> GetListPaginateAsync(int page, int size, FilterDefinition<T>? filter = null, EntitySortModelMongo<T>? sort = null, CancellationToken cancellationToken = default)
    {
        filter ??= FilterDefinition<T>.Empty;
        var query = _collection.Find(filter);
        if (sort != null)
        {
            query = query.Sort(GetSortDefinitionBuilder(sort));
        }
        var countTask = _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var itemsTask = query.Skip(page * size).Limit(size).ToListAsync(cancellationToken);
        await Task.WhenAll(countTask, itemsTask);
        return new PaginateMongo<T>
        {
            Index = page,
            Size = size,
            From = 1,
            Count = countTask.Result,
            Items = itemsTask.Result,
            Pages = (int)Math.Ceiling(countTask.Result / (double)size),
            HasPrevious = page > 1,
            HasNext = page * size < countTask.Result
        };
    }

    /* add */
    public void Add(T entity)
    {
        _collection.InsertOne(entity);
    }
    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        return _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }
    public void AddRange(IEnumerable<T> entities)
    {
        _collection.InsertMany(entities);
    }
    public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        return _collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }

    /* update */
    public ReplaceOneResult Update(Expression<Func<T, bool>> filter, T entity)
    {
        return _collection.ReplaceOne(Builders<T>.Filter.Where(filter), entity);
    }
    public Task<ReplaceOneResult> UpdateAsync(Expression<Func<T, bool>> filter, T entity, CancellationToken cancellationToken = default)
    {
        return _collection.ReplaceOneAsync(Builders<T>.Filter.Where(filter), entity, cancellationToken: cancellationToken);
    }
    public Task<UpdateResult> ExecuteUpdateAsync(Expression<Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>>> updateExpression,
        Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        var updateDefinition = updateExpression.Compile().Invoke(Builders<T>.Update);
        var filterDefinition = filter != null ? Builders<T>.Filter.Where(filter) : Builders<T>.Filter.Empty;
        return _collection.UpdateManyAsync(filterDefinition, updateDefinition, cancellationToken: cancellationToken);
    }

    /* delete */
    public DeleteResult ExecuteDelete(Expression<Func<T, bool>> filter)
    {
        return _collection.DeleteMany(Builders<T>.Filter.Where(filter));
    }
    public Task<DeleteResult> ExecuteDeleteAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return _collection.DeleteManyAsync(Builders<T>.Filter.Where(filter), cancellationToken);
    }

    /* other */
    public async Task RefreshAsync(Expression<Func<T, bool>> filter, T entity, CancellationToken cancellationToken = default)
    {
        await ExecuteDeleteAsync(filter, cancellationToken);
        await AddAsync(entity, cancellationToken);
    }
    public async Task RefreshAllAsync(Expression<Func<T, bool>> filter, IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await ExecuteDeleteAsync(filter, cancellationToken);
        await AddRangeAsync(entities, cancellationToken);
    }

    public TResult Select<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null)
    {
        return _collection.Find(filter ?? FilterDefinition<T>.Empty).Project(select).FirstOrDefault();
    }
    public Task<TResult> SelectAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null)
    {
        return _collection.Find(filter ?? FilterDefinition<T>.Empty).Project(select).FirstOrDefaultAsync();
    }
    public List<TResult> SelectAll<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null)
    {
        return _collection.Find(filter ?? FilterDefinition<T>.Empty).Project(select).ToList();
    }
    public Task<List<TResult>> SelectAllAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null)
    {
        return _collection.Find(filter ?? FilterDefinition<T>.Empty).Project(select).ToListAsync();
    }
    public int Count(Expression<Func<T, bool>>? filter = null)
    {
        return (int)_collection.CountDocuments(filter ?? FilterDefinition<T>.Empty);
    }
    public Task<long> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        return _collection.CountDocumentsAsync(filter ?? FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
    }
    public bool Any(Expression<Func<T, bool>> filter)
    {
        return _collection.Find(filter).Any();
    }
    public Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return _collection.Find(filter).AnyAsync(cancellationToken);
    }
    public TResult? Max<TResult>(Expression<Func<T, TResult>> filter)
    {
        return _collection.AsQueryable().Max(filter);
    }
    public TResult? Min<TResult>(Expression<Func<T, TResult>> filter)
    {
        return _collection.AsQueryable().Min(filter);
    }

    private static SortDefinition<T> GetSortDefinitionBuilder(EntitySortModelMongo<T> sort)
    {
        var sortDefinitionBuilder = new SortDefinitionBuilder<T>();
        return sort.IsDescending
            ? sortDefinitionBuilder.Descending(sort.Sort)
            : sortDefinitionBuilder.Ascending(sort.Sort);
    }
}

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
}

/// <summary>
/// Extension methods for configuring MongoDB services.
/// </summary>
public static class UEntityMongoDbExtension
{
    public static MongoClient? UEntityMongoClient;

    /// <summary>
    /// Configures MongoDB services and initializes the client.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="mongoClient">The MongoDB client instance to use.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddUEntityMongoDb([NotNull] this IServiceCollection services, MongoClient mongoClient)
    {
        ArgumentNullException.ThrowIfNull(services);
        UEntityMongoClient = mongoClient;
        DbMonitor();
        return services;
    }

    private static async Task DbMonitor()
    {
        while (true)
        {
            try
            {
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await UEntityMongoClient!.GetDatabase("admin").RunCommandAsync<BsonDocument>(new BsonDocument { { "ping", 1 } },
                    cancellationToken: cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n{DateTime.Now.ToString("u")} MongoDB connection failed: {e.Message}");

                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{DateTime.Now.ToString("u")} Re-establishing the MongoDB connection...");
                    UEntityMongoClient = new MongoClient(UEntityMongoClient!.Settings);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{DateTime.Now.ToString("u")} The MongoDB connection was successfully re-established.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{DateTime.Now.ToString("u")} MongoDB reconnection failure: {ex.Message}");
                }

                Console.ResetColor();
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}
public interface IMongoEntity { }
public record EntitySortModelMongo<T>
{
    public required Expression<Func<T, object?>> Sort { get; set; } = null!;
    public bool IsDescending { get; set; }
}
public record PaginateMongo<T>
{
    public int From { get; set; } = 0;
    public int Index { get; set; } = 0;
    public int Size { get; set; } = 0;
    public long Count { get; set; } = 0;
    public int Pages { get; set; } = 0;
    public List<T> Items { get; set; } = [];
    public bool HasPrevious { get; set; } = false;
    public bool HasNext { get; set; } = false;
}
