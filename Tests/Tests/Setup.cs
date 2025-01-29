using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using UEntity.MongoDb;
using System;

namespace Tests.Tests;

[SetUpFixture]
public class TestSetup
{
    private static Random _random = new();

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        UEntityMongoDbExtension.UEntityMongoClient = new MongoClient("mongodb://localhost:27017");
        Setup.DATABASE = new EntityRepositoryMongo<TestEntity>("test_db");

        //var all_agents = Enumerable.Range(0, 100).Select(x => Guid.NewGuid().ToString()).ToList();
        //var all_companies = Enumerable.Range(0, 10).Select(x => Guid.NewGuid().ToString()).ToList();

        //var entities = Enumerable.Range(1, 1000000).Select(i => new TestEntity
        //{
        //    company_id = GetRandomId(all_companies),
        //    agent_id = GetRandomId(all_agents),
        //    name = "User" + i,
        //    office = "HQ",
        //    phone = "555-000" + i,
        //    email = "user" + i + "@example.com",
        //    age = 20 + (i % 50),
        //    is_active = i % 2 == 0,
        //    created_date = GetRandomDate()
        //}).ToArray();

        //Setup.DATABASE.AddRange(entities);
    }

    private static string GetRandomId(List<string> id_list)
    {
        int randomIndex = _random.Next(0, id_list.Count);
        return id_list[randomIndex];
    }

    public static DateTime GetRandomDate(int minYear = 1900, int maxYear = 2099)
    {
        var year = _random.Next(minYear, maxYear);
        var month = _random.Next(1, 12);
        var noOfDaysInMonth = DateTime.DaysInMonth(year, month);
        var day = _random.Next(1, noOfDaysInMonth);

        return new DateTime(year, month, day);
    }

    public static TestEntity GetNewEntity()
    {
        var i = _random.Next();
        return new TestEntity
        {
            company_id = Guid.NewGuid().ToString(),
            agent_id = Guid.NewGuid().ToString(),
            name = "User" + i,
            office = "HQ",
            phone = "555-000" + i,
            email = "user" + i + "@example.com",
            age = 20 + (i % 50),
            is_active = i % 2 == 0,
            created_date = GetRandomDate()
        };
    }
}

public static class Setup
{
    public static EntityRepositoryMongo<TestEntity> DATABASE { get; set; } = null!;
}

public record TestEntity : IMongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    public string company_id { get; set; } = null!;
    public string agent_id { get; set; } = null!;
    public string name { get; set; } = null!;
    public string office { get; set; } = null!;
    public string phone { get; set; } = null!;
    public string email { get; set; } = null!;
    public int age { get; set; }
    public bool is_active { get; set; }
    public DateTime created_date { get; set; }
}

//public interface ITestEntityDal : IEntityRepositoryMongo<TestEntity> { }
//public class TestEntityDal : EntityRepositoryMongo<TestEntity>, ITestEntityDal
//{
//    public TestEntityDal() : base("test_database") { }
//}