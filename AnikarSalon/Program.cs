using AnikarSalon.DataPersistence.PostgreSQL;
using AnikarSalon.DataPersistence.PostgreSQL.Repositories;
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

app.MapGet("/system/get-masters-list", async (context) =>
{
    using (var scope = app.Services.CreateScope())
    {
        MastersRepository mastersRepo;
        var services = scope.ServiceProvider;
        mastersRepo = services.GetRequiredService<MastersRepository>();

        var mastersList = await mastersRepo.GetAll();

        if (mastersList == null) await context.Response.WriteAsync("none");
        else
        {
            StringBuilder mastersJsonBuild = new StringBuilder("{\"masters\":[");
            foreach (var master in mastersList)
            {
                string masterJson = $"{{\"name\":\"{master.FirstName}\"," +
                $"\"service\":\"{master.Services[0]}\"," +
                $"\"id\":\"{master.Id.ToString()}\"," +
                $"\"avatarUrl\":\"{master.AvatarUrl}\"}},";

                mastersJsonBuild.Append(masterJson);
            }
            mastersJsonBuild.Remove(mastersJsonBuild.Length - 1, 1);
            mastersJsonBuild.Append("]}");

            await context.Response.WriteAsync(mastersJsonBuild.ToString());
        }
    }
});

app.MapPost("/system/check-free-registration-times", async (context) =>
{
    string[] temp = { "9:00", "9:30", "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "13:30",
        "14:00", "14:30",  "15:00", "15:30", "16:00", "16:30", "17:00" };
    List<string> freeRegistrationTimes = temp.ToList();

    using (var scope = app.Services.CreateScope())
    {
        MastersRepository mastersRepo;
        var services = scope.ServiceProvider;
        mastersRepo = services.GetRequiredService<MastersRepository>();
        var requestData = context.Request.Form;

        List<string> occupiedTimes = await mastersRepo.GetFreeRegistrationTimes(
            requestData["date"].ToString(),
            requestData["masterId"].ToString());

        foreach (var occupiedTime in occupiedTimes)
        {
            freeRegistrationTimes.Remove(occupiedTime);
        }
    }

    await context.Response.WriteAsync(String.Join(",", freeRegistrationTimes.ToArray()));
});

app.MapGet("/system/get-username", async (context) =>
{
    if (context.Session.Keys.Contains("username"))
    {
        string userName = context.Session.GetString("username") ?? "Профиль";
        await context.Response.WriteAsync(userName);
    }
    else await context.Response.WriteAsync("");
});

app.MapPost("/system/get-service-price", async (context) =>
{
    decimal price;
    (_, price) = AppointmentData.Get(context.Request.Form["service"].ToString());
    await context.Response.WriteAsync(price.ToString());
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
            password: loginData["password"].ToString());

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

app.MapGet("/system/check-client-appointments", async (context) =>
{
    if (context.Session.Keys.Contains("userId"))
    {
        using (var scope = app.Services.CreateScope())
        {
            ClientsRepository clientsRepo;
            var services = scope.ServiceProvider;
            clientsRepo = services.GetRequiredService<ClientsRepository>();
            var userId = context.Session.GetString("userId");

            var appointmentsList = await clientsRepo.GetAllAppointments(userId ?? "");

            StringBuilder appointmentsJsonBuild = new StringBuilder("{\"appointments\":[");
            foreach (var appointment in appointmentsList)
            {
                string date = appointment.DateTime.Year.ToString() + '-'
                + (appointment.DateTime.Month < 10 ? '0' + appointment.DateTime.Month.ToString() : appointment.DateTime.Month) + '-'
                + (appointment.DateTime.Day < 10 ? '0' + appointment.DateTime.Day.ToString() : appointment.DateTime.Day) + ' '
                + appointment.DateTime.Hour.ToString() + ':'
                + (appointment.DateTime.Minute < 10 ? '0' + appointment.DateTime.Minute.ToString() : appointment.DateTime.Minute);

                string appointmentJson = $"{{\"appointmentId\":\"{appointment.Id}\"," +
                $"\"master\":\"{appointment.Master.FirstName}\"," +
                $"\"service\":\"{appointment.AppointmentName}\"," +
                $"\"price\":\"{appointment.Price}\"," +
                $"\"date\":\"{date}\"," +
                $"\"status\":\"{appointment.Status}\"}},";

                appointmentsJsonBuild.Append(appointmentJson);
            }
            if (appointmentsList.Count > 0) appointmentsJsonBuild.Remove(appointmentsJsonBuild.Length - 1, 1);
            appointmentsJsonBuild.Append("]}");

            await context.Response.WriteAsync(appointmentsJsonBuild.ToString());
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

                if (context.Request.Cookies.ContainsKey("chosenDate")
                || context.Request.Cookies.ContainsKey("chosenTime")
                || context.Request.Cookies.ContainsKey("chosenMaster")
                || context.Request.Cookies.ContainsKey("chosenService"))
                    context.Response.Redirect("/registration");

                else context.Response.Redirect("/profile");
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

            if (context.Request.Cookies.ContainsKey("chosenDate")
            || context.Request.Cookies.ContainsKey("chosenTime")
            || context.Request.Cookies.ContainsKey("chosenMaster")
            || context.Request.Cookies.ContainsKey("chosenService"))
                context.Response.Redirect("/registration");

            else context.Response.Redirect("/profile");
        }
    }

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/signup.html");
});

app.MapGet("/profile", async (context) =>
{
    if (!context.Session.Keys.Contains("userId")) context.Response.Redirect("/login");
    else
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.SendFileAsync("wwwroot/clientProfile.html");
    }
});

app.MapGet("/registration", async (context) =>
{
    if (context.Request.Query.Count == 4)
    {
        using (var scope = app.Services.CreateScope())
        {
            AppointmentsRepository appointmentRepo;
            var services = scope.ServiceProvider;
            appointmentRepo = services.GetRequiredService<AppointmentsRepository>();
            var registrationData = context.Request.Query;
            if (context.Session.Keys.Contains("userId"))
            {
                Console.WriteLine(await appointmentRepo.Registrate(
                    registrationData["chosenDate"].ToString(),
                    registrationData["chosenTime"].ToString(),
                    context.Session.GetString("userId") ?? "",
                    registrationData["chosenMaster"].ToString(),
                    registrationData["chosenService"].ToString()));

                if (context.Request.Cookies.ContainsKey("chosenDate")) context.Response.Cookies.Delete("chosenDate");
                if (context.Request.Cookies.ContainsKey("chosenTime")) context.Response.Cookies.Delete("chosenTime");
                if (context.Request.Cookies.ContainsKey("chosenMaster")) context.Response.Cookies.Delete("chosenMaster");
                if (context.Request.Cookies.ContainsKey("chosenService")) context.Response.Cookies.Delete("chosenService");

                context.Response.Redirect("/");
            }
            else
            {
                context.Response.Cookies.Append("chosenService", registrationData["chosenService"].ToString());

                context.Response.Redirect("/login");
            }
        }
    }
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/appointmentRegistration.html");
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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