using AnikarSalon.DataPersistence.PostgreSQL.Repositories;
using AnikarSalon.Persistence.Postgres.Models;
using System.Text;

namespace AnikarSalon.MapMethods
{
    public static class MapClient
    {
        public static async Task Index(HttpContext context, WebApplication app)
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync("wwwroot/index.html");
        }

        public static async Task Login(HttpContext context, WebApplication app)
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
                        phoneNumber: AdditionalMethods.PhoneBuilder(loginData["userPhone"].ToString()),
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
        }

        public static async Task SignUp(HttpContext context, WebApplication app)
        {
            if (context.Request.Query.Count > 0)
            {
                using (var scope = app.Services.CreateScope())
                {
                    ClientsRepository clientsRepo;
                    var services = scope.ServiceProvider;
                    clientsRepo = services.GetRequiredService<ClientsRepository>();
                    var userData = context.Request.Query;

                    string phoneNumber = AdditionalMethods.PhoneBuilder(userData["userPhone"].ToString());

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
        }

        public static async Task Profile(HttpContext context, WebApplication app)
        {
            if (!context.Session.Keys.Contains("userId")) context.Response.Redirect("/login");
            else
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.SendFileAsync("wwwroot/clientProfile.html");
            }
        }

        public static async Task Registration(HttpContext context, WebApplication app)
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
        }
    }
}
