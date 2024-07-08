using AnikarSalon.DataPersistence.PostgreSQL.Repositories;
using AnikarSalon.Persistence.Postgres;
using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "SessionId";
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.IsEssential = true;
});

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

app.UseSession();

app.MapGet("/system/get-username", async (context) =>
{
    if (context.Session.Keys.Contains("username"))
    {
        string userName = context.Session.GetString("username") ?? "Профиль";
        await context.Response.WriteAsync(userName);
    }
    else await context.Response.WriteAsync("");
});

app.MapPost("/system/check-login", async (context) =>
{
    using (var scope = app.Services.CreateScope())
    {
        ClientsRepository clientsRepo;
        var services = scope.ServiceProvider;
        clientsRepo = services.GetRequiredService<ClientsRepository>();
        var loginData = context.Request.Form;

        ClientEntity? user = await clientsRepo.Login(
            phoneNumber: PhoneBuilder(loginData["userPhone"].ToString()),
            password: loginData["password"]);

        if (user != null)
        {
            await context.Response.WriteAsync("true");
        }
        else
        {
            await context.Response.WriteAsync("false");
        }
    }
});

app.MapPost("/system/is-number-occupied", async (context) =>
{
    using (var scope = app.Services.CreateScope())
    {
        ClientsRepository clientsRepo;
        var services = scope.ServiceProvider;
        clientsRepo = services.GetRequiredService<ClientsRepository>();
        var signupData = context.Request.Form;

        string phoneNumber = PhoneBuilder(signupData["userPhone"].ToString());

        if (await clientsRepo.IsNumberOccupied(phoneNumber))
        {
            await context.Response.WriteAsync("true");
            Console.WriteLine("true");
        }
        else
        {
            await context.Response.WriteAsync("false");
            Console.WriteLine("false");
        }
    }
});

app.MapGet("/", async (context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/index.html");
});

app.MapGet("/login", async (context) =>
{
    if (context.Request.Query.Count > 0)
    {
        using (var scope = app.Services.CreateScope())
        {
            ClientsRepository clientsRepo;
            var services = scope.ServiceProvider;
            clientsRepo = services.GetRequiredService<ClientsRepository>();
            var loginData = context.Request.Query;

            ClientEntity? user = await clientsRepo.Login(
                phoneNumber: PhoneBuilder(loginData["userPhone"].ToString()),
                password: loginData["password"].ToString());

            if (user != null)
            {
                context.Session.SetString("username", user.FirstName ?? "");
                context.Session.SetString("userId", user.Id.ToString() ?? "");
                context.Response.Redirect("/");
            }
            else
            {
                context.Response.Redirect("/login");
            }
        }
    }

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/login.html");
});

app.MapGet("/signup", async (context) =>
{
    if (context.Request.Query.Count > 0)
    {
        using (var scope = app.Services.CreateScope())
        {
            ClientsRepository clientsRepo;
            var services = scope.ServiceProvider;
            clientsRepo = services.GetRequiredService<ClientsRepository>();
            var userData = context.Request.Query;

            string phoneNumber = PhoneBuilder(userData["userPhone"].ToString());

            ClientEntity newClient = new ClientEntity()
            {
                FirstName = userData["firstname"].ToString(),
                LastName = userData["lastname"].ToString(),
                Password = userData["password"].ToString(),
                PhoneNumber = phoneNumber
            };
            await clientsRepo.Add(newClient);

            context.Session.SetString("username", newClient.FirstName ?? "");
            context.Session.SetString("userId", newClient.Id.ToString() ?? "");

            context.Response.Redirect("/");
        }
    }

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/signup.html");
});

app.Run();

string PhoneBuilder(string phoneNumber)
{
    StringBuilder phoneNumberBuilder = new(phoneNumber);
    while (phoneNumberBuilder.Length != 11)
    {
        char[] symbolsToDelete = { ' ', '-', '+', '(', ')', ':', '_' };
        foreach (char symbol in symbolsToDelete)
            phoneNumberBuilder.Replace(symbol.ToString(), "");
    }
    phoneNumber = phoneNumberBuilder.ToString();
    phoneNumber = phoneNumber.Remove(0, 1).Insert(0, "8");

    return phoneNumber;
}