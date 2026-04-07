using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace UEntity.MongoDb;

public interface IEntityRepositoryMongo<T, IBaseEntity> where T : class, IBaseEntity, new()
{
    IMongoCollection<T> Collection();

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

    Task<PaginateMongo<TResult>> GetAggregatePaginateAsync<TResult>(
            int page, int size,
            IList<BsonDocument> basePipeline,
            CancellationToken cancellationToken = default);

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
    Task<IAsyncCursor<BsonDocument>> AggregateAsync(BsonDocument[] pipeline);
}