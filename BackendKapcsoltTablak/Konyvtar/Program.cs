using Konyvtar.Adatok;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var adatbazis = Path.Combine(builder.Environment.ContentRootPath, "konyvtar.db");
builder.Services.AddDbContext<KonyvtarContext>(options =>
    options.UseSqlite($"Data Source={adatbazis}"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<KonyvtarContext>().Database.EnsureCreated();
}

app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    Alkalmazas = "Könyvtári kölcsönzés API",
    Vegpontok = new[] { "/api/olvasok", "/api/konyvek", "/api/konyvek/szabad", "/api/olvasok/kolcsonzok" }
}));
app.Run();
