using Microsoft.EntityFrameworkCore;
using Minimal_API2.Model;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/*var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}*/

app.MapGet("/", () => "Hello World!");
app.MapGet("/items/{id}", (Guid id, IItemRepository itemRepository) =>
{
    return itemRepository.Get(id);
});
app.MapGet("/items", (IItemRepository itemRepository) =>
{
    return itemRepository.GetAll();
});
app.MapPost("/items", (Item item, IItemRepository itemRepository) =>
{
    itemRepository.Add(item);
});
app.MapPut("/items", (Item item, IItemRepository itemRepository) =>
{
    itemRepository.Update(item);
});
app.MapDelete("/items/{id}", (Guid id, IItemRepository itemRepository) =>
{
    var item = itemRepository.Get(id);
    itemRepository.Delete(item);
});

//Get All Students
app.MapGet("/minimalapi/Students", (AppDbContext db) =>
{
    return db.Students.ToList();
});
//Get All students By Id
app.MapGet("/minimalApi/StudentsById", (AppDbContext db, int id) =>
{
    var students = db.Students.Find(id);
    return Results.Ok(students);
});
//Add students
app.MapPost("/minimalApi/Addstudents", (AppDbContext db, Student stud) =>
{
    db.Students.Add(stud);
    db.SaveChanges();
    return Results.Created($"/minimalApi/StudentsById/{ stud.Id}", stud);
});
//Update students
app.MapPut("/minimalApi/Updatestudents/", (AppDbContext db, Student stud) =>
{
    var students = db.Students.FirstOrDefault(x => x.Id == stud.Id);
    students.Name = stud.Name;
    students.Age = stud.Age;
    db.Students.Update(students);
    db.SaveChanges();
    return Results.NoContent();
});
//Delete students
app.MapDelete("/minimalApi/Deltestudents/", (AppDbContext db, int id) =>
{
    var students = db.Students.Find(id);
    db.Students.Remove(students);
    db.SaveChanges();
    return Results.NoContent();
});

app.Run();
public class Item
{
    public Item(string description, int quantity)
    {
        Description = description;
        Quantity = quantity;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public string Description { get; set; }

    public int Quantity { get; set; }
}

public interface IItemRepository
{
    void Add(Item item);
    void Delete(Item item);
    Item Get(Guid id);
    void Update(Item item);
    IEnumerable<Item> GetAll();
}

public class ItemRepository : IItemRepository
{
    private readonly IList<Item> _items;
    public ItemRepository()
    {
        _items = new List<Item>();
    }

    public void Add(Item item)
    {
        _items.Add(item);
    }

    public void Delete(Item item)
    {
        _items.Remove(item);
    }

    public Item Get(Guid id)
    {
        return _items.Single(item => item.Id == id);
    }

    public void Update(Item item)
    {
        var listItem = _items.Single(item => item.Id == item.Id);

        listItem.Description = item.Description;
        listItem.Quantity = item.Quantity;
    }

    public IEnumerable<Item> GetAll()
    {
        return _items;
    }
}

