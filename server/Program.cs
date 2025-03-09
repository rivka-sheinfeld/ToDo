using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TodoApi;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
    builder => builder.WithOrigins("http://localhost:3000")
    .AllowAnyMethod()
    .AllowAnyHeader());
});
builder.Services.AddDbContext<ToDoDbContext>(options=>options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
new MySqlServerVersion(new Version(8,0,0))));
var app = builder.Build();
//הפעלת הCORS
app.UseCors("AllowSpecificOrigin");
// app.MapGet("/", () => "This is a GET");
app.MapGet("/", async(ToDoDbContext db) =>
{
   return await db.Items.ToListAsync();
});
app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});

app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null)
    {
        return Results.NotFound();
    }

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
// app.MapPut("/items/{id}", async (ToDoDbContext db, int id, Item updatedItem) =>
// {
//     if (id != updatedItem.Id)
//     {
//         return Results.BadRequest();
//     }

//     var existingItem = await db.Items.FindAsync(id);
//     if (existingItem is null)
//     {
//         return Results.NotFound();
//     }

//     existingItem.Name = updatedItem.Name;
//     existingItem.IsComplete = updatedItem.IsComplete;

//     db.Items.Update(existingItem);
//     await db.SaveChangesAsync();

//     return Results.NoContent();
// });
app.MapPut("/updateItem", async (ToDoDbContext context, int id, bool? IsComplete) =>
{
    var item = await context.Items.FindAsync(id);
    if (item is null)
    {
        return Results.NotFound();
    }

    if (item.Id == id)
    {
        if (IsComplete != null)
        {
            item.IsComplete = IsComplete;
        }
    }

    await context.SaveChangesAsync();
    return Results.NoContent();

});


app.MapMethods("/options-or-head", new[] { "OPTIONS", "HEAD" }, 
                          () => "This is an options or head request ");


app.Run();

