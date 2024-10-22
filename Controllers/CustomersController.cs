using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDbCRUD.Data;
using MongoDbCRUD.Entities;

namespace MongoDbCRUD.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly IMongoCollection<Customer> _customers;

    public CustomersController(MongoDbService mongoDbService)
    {
        _customers = mongoDbService.Database?.GetCollection<Customer>("customer");
    }

    [HttpGet]
    public async Task<IEnumerable<Customer>> Get()
    {
        return await _customers.Find(FilterDefinition<Customer>.Empty).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(string id)
    {
        var filter = Builders<Customer>.Filter.Eq(p => p.Id, id);
        var customer = _customers.Find(filter).FirstOrDefault();
        return customer is not null ? Ok(customer) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> Post(Customer customer)
    {
        await _customers.InsertOneAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut]
    public async Task<ActionResult> Put(Customer customer)
    {
        var filter = Builders<Customer>.Filter.Eq(p => p.Id, customer.Id);

        //var update = Builders<Customer>.Update
        //    .Set(p => p.Name, customer.Name)
        //    .Set(p => p.Email, customer.Email);
        //await _customers.UpdateOneAsync(filter, update);

        await _customers.ReplaceOneAsync(filter, customer);
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(string id)
    {
        var filter = Builders<Customer>.Filter.Eq(p => p.Id, id);
        await _customers.DeleteOneAsync(filter);
        return Ok();
    }
}
