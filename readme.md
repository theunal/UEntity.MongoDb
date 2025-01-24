# UEntity.MongoDb NuGet Package

## Introduction
The `UEntity.MongoDb` NuGet package provides a flexible and efficient repository pattern implementation for MongoDB. This package simplifies common MongoDB operations such as querying, inserting, updating, deleting, and pagination.

## Getting Started

### Installation
Install the package via NuGet:
```bash
Install-Package UEntity.MongoDb
```

### Configuration
To use the package, configure it in your `Program.cs` file by calling the `AddUEntityMongoDb` extension method.

```csharp
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using UEntity.MongoDb;

var services = new ServiceCollection();
var mongoClient = new MongoClient("YourMongoDbConnectionString");
services.AddUEntityMongoDb(mongoClient);
```

## Usage

### Repository Methods
The `IEntityRepositoryMongo<T>` interface provides the following methods for MongoDB operations:

#### **1. Get**
Retrieves a single document based on a filter.

```csharp
var repository = new EntityRepositoryMongo<MyEntity>("DatabaseName");
var result = repository.Get(x => x.Id == "12345");
```

#### **2. GetAsync**
Asynchronously retrieves a single document based on a filter.

```csharp
var result = await repository.GetAsync(x => x.Id == "12345");
```

#### **3. GetAll**
Retrieves all documents matching a filter.

```csharp
var results = repository.GetAll(x => x.IsActive);
```

#### **4. GetAllAsync**
Asynchronously retrieves all documents matching a filter.

```csharp
var results = await repository.GetAllAsync(x => x.IsActive);
```

#### **5. GetListPaginate**
Retrieves paginated results.

```csharp
var paginatedResults = repository.GetListPaginate(0, 10, filter: null, sort: null);
```

#### **6. Add**
Inserts a single document into the collection.

```csharp
var entity = new MyEntity { Id = "12345", Name = "Test" };
repository.Add(entity);
```

#### **7. AddAsync**
Asynchronously inserts a single document into the collection.

```csharp
await repository.AddAsync(entity);
```

#### **8. Update**
Updates a document based on a filter.

```csharp
var updateResult = repository.Update(x => x.Id == "12345", updatedEntity);
```

#### **9. Delete**
Deletes a document based on a filter.

```csharp
var deleteResult = repository.Delete(x => x.Id == "12345");
```

## Advanced Topics

### Sorting
Use the `EntitySortModel<T>` to define sorting criteria:

```csharp
var sortModel = new EntitySortModel<MyEntity>
{
    Sort = x => x.Name,
    IsDescending = false
};
var results = repository.GetAll(x => x.IsActive, sortModel);
```

### Pagination
Efficiently retrieve paginated data with filters and sorting:

```csharp
var paginatedData = await repository.GetListPaginateAsync(0, 10, filter: null, sort: sortModel);
```

## Conclusion
The `UEntity.MongoDb` package simplifies MongoDB operations while offering flexibility for advanced use cases. Integrate it into your projects to streamline database operations and enhance maintainability.

For additional support or contributions, refer to the repository's documentation or reach out to the maintainers.