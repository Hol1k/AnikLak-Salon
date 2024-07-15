using AnikarSalon.DataPersistence.PostgreSQL;
using AnikarSalon.DataPersistence.PostgreSQL.Repositories;
using AnikarSalon.MapMethods;
using AnikarSalon.Persistence.Postgres;
using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "SessionId";
    options.IdleTimeout = TimeSpan.FromDays(1);
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

//System methods
app.MapGet("/system/get-masters-list", async (context) =>
await MapSystem.GetMastersList(context, app));

app.MapPost("/system/check-free-registration-times", async (context) =>
await MapSystem.CheckFreeRegistrationTimes(context, app));

app.MapGet("/system/get-mastername", async (context) =>
await MapSystem.GetMasterName(context, app));

app.MapGet("/system/get-username", async (context) =>
await MapSystem.GetUserName(context, app));

app.MapPost("/system/get-service-price", async (context) =>
await MapSystem.GetServicePrice(context, app));

app.MapPost("/system/check-master-login", async (context) =>
await MapSystem.CheckMasterLogin(context, app));

app.MapPost("/system/check-login", async (context) =>
await MapSystem.CheckLogin(context, app));

app.MapPost("/system/is-number-occupied", async (context) =>
await MapSystem.IsNumberOccupied(context, app));

app.MapGet("/system/check-client-appointments", async (context) =>
await MapSystem.CheckClientAppointments(context, app));

app.MapGet("/system/check-master-appointments", async (context) =>
await MapSystem.CheckMasterAppointments(context, app));

app.MapPost("/system/get-master-info", async (context) =>
await MapSystem.GetMasterInfo(context, app));

app.MapPost("/system/update-master-info", async (context) =>
await MapSystem.UpdateMasterInfo(context, app));

//Client methods
app.MapGet("/", async (context) =>
await MapClient.Index(context, app));

app.MapGet("/login", async (context) =>
await MapClient.Login(context, app));

app.MapGet("/signup", async (context) =>
await MapClient.SignUp(context, app));

app.MapGet("/profile", async (context) =>
await MapClient.Profile(context, app));

app.MapGet("/registration", async (context) =>
await MapClient.Registration(context, app));

//MasterMethods
app.MapGet("/master", async (context) =>
await MapMaster.Index(context, app));

app.MapGet("/master/login", async (context) =>
await MapMaster.Login(context, app));

app.MapGet("/master/logout", async (context) =>
await MapMaster.LogOut(context, app));

app.MapGet("/master/profile", async (context) =>
await MapMaster.Profile(context, app));

app.MapGet("/master/appointments", async (context) =>
await MapMaster.Appointments(context, app));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.Run();