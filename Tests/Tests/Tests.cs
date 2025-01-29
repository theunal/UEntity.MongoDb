using NUnit.Framework;
using UEntity.MongoDb;

namespace Tests.Tests;

[TestFixture]
public class Tests
{
    private const string company_id = "39d37884-6422-40db-8b7a-964fe42e4219";
    private const string agent_id = "b90355ed-977c-4673-b44f-9b334276c7da";
    private static Random _random = new();

    [Test]
    public async Task Selects()
    {
        var item = Setup.DATABASE.Get(x => x.agent_id == "_");
        Assert.That(item, Is.Null);

        item = await Setup.DATABASE.GetAsync(x => x.agent_id == "_");
        Assert.That(item, Is.Null);

        item = Setup.DATABASE.Get(x => x.company_id == company_id);
        Assert.That(item, Is.Not.Null);

        item = await Setup.DATABASE.GetAsync(x => x.company_id == company_id);
        Assert.That(item, Is.Not.Null);

        item = Setup.DATABASE.Get(x => x.company_id == company_id && x.agent_id == agent_id);
        Assert.That(item, Is.Not.Null);

        item = await Setup.DATABASE.GetAsync(x => x.company_id == company_id && x.agent_id == agent_id);
        Assert.That(item, Is.Not.Null);

        var date = DateTime.Now;
        var date2 = DateTime.Now.AddYears(-10);
        var items = await Setup.DATABASE.GetAllAsync(x => x.company_id == company_id && x.agent_id == agent_id && x.created_date < date && x.created_date > date2);
        Assert.That(items, Has.Count.EqualTo(44));

        items = Setup.DATABASE.GetAll(x => x.company_id == company_id && x.agent_id == agent_id && x.created_date < date && x.created_date > date2);
        Assert.That(items, Has.Count.EqualTo(44));

        var count = await Setup.DATABASE.CountAsync(x => x.company_id == company_id && x.agent_id == agent_id && x.created_date < date && x.created_date > date2);
        Assert.That(count, Is.EqualTo(44));

        count = Setup.DATABASE.Count(x => x.company_id == company_id && x.agent_id == agent_id && x.created_date < date && x.created_date > date2);
        Assert.That(count, Is.EqualTo(44));

        var predicate = PredicateBuilderMongo.NewQuery<TestEntity>(x => x.company_id == company_id);
        items = Setup.DATABASE.GetAll(predicate);
        Assert.That(items, Has.Count.EqualTo(100240));

        items = await Setup.DATABASE.GetAllAsync(predicate);
        Assert.That(items, Has.Count.EqualTo(100240));

        predicate = predicate.And(x => x.agent_id == agent_id);
        items = Setup.DATABASE.GetAll(predicate);
        Assert.That(items, Has.Count.EqualTo(1006));

        items = await Setup.DATABASE.GetAllAsync(predicate);
        Assert.That(items, Has.Count.EqualTo(1006));

        predicate = predicate.And(x => x.age == 21);
        items = Setup.DATABASE.GetAll(predicate);
        Assert.That(items, Has.Count.EqualTo(15));

        items = await Setup.DATABASE.GetAllAsync(predicate);
        Assert.That(items, Has.Count.EqualTo(15));

        var search_text = "10";
        predicate = predicate.And(x =>
           x.email != null && x.email.ToLower().Contains(search_text) ||
           x.name != null && x.name.ToLower().Contains(search_text));
        items = Setup.DATABASE.GetAll(predicate);
        Assert.That(items, Has.Count.EqualTo(1));

        items = await Setup.DATABASE.GetAllAsync(predicate);
        Assert.That(items, Has.Count.EqualTo(1));

        var search_or_predicate = predicate.Or(x => x.email.ToLower().Contains("18") && x.is_active == false);
        items = Setup.DATABASE.GetAll(search_or_predicate);
        Assert.That(items, Has.Count.EqualTo(19851));

        items = await Setup.DATABASE.GetAllAsync(search_or_predicate);
        Assert.That(items, Has.Count.EqualTo(19851));
    }

    [Test]
    public async Task Add()
    {
        var count = await Setup.DATABASE.CountAsync();

        await Setup.DATABASE.AddAsync(TestSetup.GetNewEntity());
        count++;

        var new_count = await Setup.DATABASE.CountAsync();
        Assert.That(new_count, Is.EqualTo(count));

        Setup.DATABASE.Add(TestSetup.GetNewEntity());
        count++;

        new_count = Setup.DATABASE.Count();
        Assert.That(new_count, Is.EqualTo(count));

        var quantity = 500;
        var new_items = Enumerable.Range(0, quantity).Select(x => TestSetup.GetNewEntity());
        Setup.DATABASE.AddRange(new_items);
        count += quantity;

        new_count = Setup.DATABASE.Count();
        Assert.That(new_count, Is.EqualTo(count));

        await Setup.DATABASE.AddRangeAsync(new_items);
        count += quantity;

        new_count = Setup.DATABASE.Count();
        Assert.That(new_count, Is.EqualTo(count));
    }

    [Test]
    public async Task Update()
    {
        var all_item_ids = await Setup.DATABASE.SelectAllAsync(x => x.Id);
        var all_item_ids2 = Setup.DATABASE.SelectAll(x => x.Id);

        Assert.That(all_item_ids2, Is.EqualTo(all_item_ids));

        int randomIndex = _random.Next(0, all_item_ids.Count);
        var item = await Setup.DATABASE.GetAsync(x => x.Id == all_item_ids[randomIndex]);
        Assert.That(item, Is.Not.Null);

        item.is_active = !item.is_active;
        item.name = Guid.NewGuid().ToString();

        var item_id = item.Id.ToString();
        var new_active_value = item.is_active;
        var new_name = item.name;

        await Setup.DATABASE.UpdateAsync(x => x.Id == item.Id, item);
        item = Setup.DATABASE.Get(x => x.name == new_name && x.is_active == new_active_value);
        Assert.That(item, Is.Not.Null);
        Assert.That(item.Id.ToString(), Is.EqualTo(item_id));
    }

    [Test]
    public async Task Delete()
    {
        var all_item_ids = await Setup.DATABASE.SelectAllAsync(x => x.Id);
        var count = all_item_ids.Count;
        var deleted_count = 750;
        var deleted_item_ids = all_item_ids.Take(deleted_count).ToList();

        var delete_response = await Setup.DATABASE.ExecuteDeleteAsync(x => deleted_item_ids.Contains(x.Id));
        count -= deleted_count;
        var new_count = await Setup.DATABASE.CountAsync();
        Assert.Multiple(() =>
        {
            Assert.That(new_count, Is.EqualTo(count));
            Assert.That(deleted_count, Is.EqualTo(delete_response.DeletedCount));
        });

        all_item_ids.RemoveAll(deleted_item_ids.Contains);

        deleted_count = 1;
        deleted_item_ids = all_item_ids.Take(deleted_count).ToList();
        delete_response = Setup.DATABASE.ExecuteDelete(x => deleted_item_ids.Contains(x.Id));
        count -= deleted_count;
        new_count = await Setup.DATABASE.CountAsync();
        Assert.Multiple(() =>
        {
            Assert.That(new_count, Is.EqualTo(count));
            Assert.That(deleted_count, Is.EqualTo(delete_response.DeletedCount));
        });
    }
}