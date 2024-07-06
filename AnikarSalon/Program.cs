using AnikarSalon.DataPersistence.PostgreSQL.Repositories;
using AnikarSalon.Persistence.Postgres;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SalonDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(SalonDbContext)));
});

builder.Services.AddScoped<AppointmentsRepository>();
builder.Services.AddScoped<ClientsRepository>();
builder.Services.AddScoped<CommentsRepository>();
builder.Services.AddScoped<MastersRepository>();
builder.Services.AddScoped<OrdersRepository>();

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/", async (context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/index.html");
});

app.MapGet("/login", async (context) =>
{
    if (context.Request.Query.Count > 0) context.Response.Redirect("/");

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/login.html");
});

app.MapGet("/signup", async (context) =>
{
    if (context.Request.Query.Count > 0) context.Response.Redirect("/");

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/signup.html");
});

app.Run();

RepoType GetRepo<RepoType>() where RepoType : notnull
{
    RepoType repository;
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        repository = services.GetRequiredService<RepoType>();
    }
    return repository;
}