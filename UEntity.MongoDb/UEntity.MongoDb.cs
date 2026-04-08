
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace UEntity.MongoDb;

public class EntityRepositoryMongo<T, IBaseEntity>(string databaseName) : IEntityRepositoryMongo<T, IBaseEntity> where T : class, IBaseEntity, new()
{
    private readonly IMongoCollection<T> _collection = UEntityMongoDbExtension.UEntityMongoClient?.GetDatabase(databaseName)?.GetCollection<T>(typeof(T).Name) ??
        throw new ArgumentNullException("Please call the “AddUEntityMongoDb” method in your program record.");

    public IMongoCollection<T> Collection() => _collection;

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

    public Task<IAsyncCursor<BsonDocument>> AggregateAsync(BsonDocument[] pipeline)
    {
        return _collection.AggregateAsync<BsonDocument>(pipeline);
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
        page = page < 1 ? 1 : page;
        size = size <= 0 ? 5 : size;
        filter ??= FilterDefinition<T>.Empty;
        var query = _collection.Find(filter);
        if (sort != null)
        {
            query = query.Sort(GetSortDefinitionBuilder(sort));
        }
        var total_count = (int)_collection.CountDocuments(filter);
        var items = query.Skip((page - 1) * size).Limit(size).ToList();
        var pages_count = (int)Math.Ceiling(total_count / (double)size);
        return new PaginateMongo<T>
        {
            Page = page,
            Size = size,
            Items = items,
            TotalCount = total_count,
            PagesCount = pages_count,
            HasPrevious = page > 1,
            HasNext = page < pages_count
        };
    }
    public async Task<PaginateMongo<T>> GetListPaginateAsync(
        int page, int size,
        FilterDefinition<T>? filter = null,
        EntitySortModelMongo<T>? sort = null,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        size = size <= 0 ? 5 : size;

        filter ??= FilterDefinition<T>.Empty;
        var query = _collection.Find(filter);
        if (sort != null)
        {
            query = query.Sort(GetSortDefinitionBuilder(sort));
        }
        var countTask = _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var itemsTask = query.Skip((page - 1) * size).Limit(size).ToListAsync(cancellationToken);
        await Task.WhenAll(countTask, itemsTask);
        var total_count = countTask.Result;
        var pages_count = (int)Math.Ceiling(total_count / (double)size);
        return new PaginateMongo<T>
        {
            Page = page,
            Size = size,
            Items = itemsTask.Result,
            TotalCount = total_count,
            PagesCount = pages_count,
            HasPrevious = page > 1,
            HasNext = page < pages_count
        };
    }
    public async Task<PaginateMongo<TResult>> GetAggregatePaginateAsync<TResult>(
        int page, int size,
        IList<BsonDocument> basePipeline,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        size = size <= 0 ? 5 : size;
        int skip = (page - 1) * size;

        var countTask = _collection
            .Aggregate<BsonDocument>(new List<BsonDocument>(basePipeline)
        {
            new("$count", "TotalCount")
        }, cancellationToken: cancellationToken).FirstOrDefaultAsync(cancellationToken);

        // Base pipeline’ın sonuna skip/limit aşamaları ekleniyor
        var itemsTask = _collection
            .Aggregate<TResult>(new List<BsonDocument>(basePipeline)
        {
            new("$skip", skip),
            new("$limit", size)
        }, cancellationToken: cancellationToken).ToListAsync(cancellationToken);

        await Task.WhenAll(countTask, itemsTask);

        int total_count = countTask.Result?["TotalCount"].AsInt32 ?? 0;
        int pages_count = (int)Math.Ceiling(total_count / (double)size);
        return new PaginateMongo<TResult>
        {
            Page = page,
            Size = size,
            Items = itemsTask.Result,
            TotalCount = total_count,
            PagesCount = pages_count,
            HasPrevious = page > 1,
            HasNext = page < pages_count
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
        if (entity == null) return;

        await ExecuteDeleteAsync(filter, cancellationToken);
        await AddAsync(entity, cancellationToken);
    }
    public async Task RefreshAllAsync(Expression<Func<T, bool>> filter, IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null || !entities.Any()) return;

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

    //private static SortDefinition<T> GetSortDefinitionBuilder(EntitySortModelMongo<T> sort)
    //{
    //    //var sortDefinitionBuilder = new SortDefinitionBuilder<T>();
    //    //return sort.IsDescending
    //    //    ? sortDefinitionBuilder.Descending(sort.Sort)
    //    //    : sortDefinitionBuilder.Ascending(sort.Sort);

    //    var sortDefinitionBuilder = new SortDefinitionBuilder<T>();

    //    var sortDefinition = sort.IsDescending
    //        ? sortDefinitionBuilder.Descending(sort.Sort)
    //        : sortDefinitionBuilder.Ascending(sort.Sort);

    //    // İkincil sıralama olarak _id ekle (stabil pagination için)
    //    sortDefinition = sort.IsDescending
    //        ? sortDefinition.Descending("_id")
    //        : sortDefinition.Ascending("_id");

    //    return sortDefinition;
    //}

    private static SortDefinition<T> GetSortDefinitionBuilder(EntitySortModelMongo<T> sort)
    {
        var sortDefinitionBuilder = new SortDefinitionBuilder<T>();

        // Birincil sıralama
        var sortDefinition = sort.IsDescending
            ? sortDefinitionBuilder.Descending(sort.Sort)
            : sortDefinitionBuilder.Ascending(sort.Sort);

        // İkincil, üçüncül vs. sıralamalar
        foreach (var (Field, IsDescending) in sort.AdditionalSorts ?? [])
        {
            sortDefinition = IsDescending
                ? sortDefinition.Descending(Field)
                : sortDefinition.Ascending(Field);
        }

        // Son olarak _id ekle (stabil pagination için)
        sortDefinition = sort.IsDescending
            ? sortDefinition.Descending("_id")
            : sortDefinition.Ascending("_id");

        return sortDefinition;
    }
}