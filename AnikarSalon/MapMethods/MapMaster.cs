using AnikarSalon.DataPersistence.PostgreSQL.Repositories;
using AnikarSalon.Persistence.Postgres.Models;

namespace AnikarSalon.MapMethods
{
    public static class MapMaster
    {
        public static async Task Index(HttpContext context, WebApplication app)
        {
            if (!context.Session.Keys.Contains("masterId")) context.Response.Redirect("/master/login");
            else context.Response.Redirect("/master/appointments");
            await Task.Yield();
        }

        public static async Task Login(HttpContext context, WebApplication app)
        {
            if (context.Request.Query.Count > 0)
            {
                using (var scope = app.Services.CreateScope())
                {
                    MastersRepository mastersRepo;
                    var services = scope.ServiceProvider;
                    mastersRepo = services.GetRequiredService<MastersRepository>();
                    var loginData = context.Request.Query;

                    MasterEntity? user = await mastersRepo.Login(
                        phoneNumber: AdditionalMethods.PhoneBuilder(loginData["userPhone"].ToString()),
                        password: loginData["password"].ToString());

                    if (user != null)
                    {
                        context.Session.SetString("masterName", user.FirstName ?? "");
                        context.Session.SetString("masterId", user.Id.ToString() ?? "");

                        context.Response.Redirect("/master/appointments");
                    }
                    else
                    {
                        context.Response.Redirect("/master/login");
                    }
                }
            }

            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync("wwwroot/master/masterLogin.html");
        }

        public static async Task LogOut(HttpContext context, WebApplication app)
        {
            context.Session.Remove("masterId");
            context.Session.Remove("masterName");

            context.Response.Redirect("/master/login");
            await Task.Yield();
        }

        public static async Task Appointments(HttpContext context, WebApplication app)
        {
            if (context.Request.Query.Count > 0)
            {
                using (var scope = app.Services.CreateScope())
                {
                    AppointmentsRepository appointmentsRepo;
                    var services = scope.ServiceProvider;
                    appointmentsRepo = services.GetRequiredService<AppointmentsRepository>();
                    var requestData = context.Request.Query;

                    await appointmentsRepo.UpdateStatus(
                        requestData["appointId"].ToString(),
                        requestData["status"].ToString());

                    context.Response.Redirect("/master/appointments");
                }
            }
            else
            {
                if (!context.Session.Keys.Contains("masterId")) context.Response.Redirect("/master");
                else
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.SendFileAsync("wwwroot/master/masterAppointments.html");
                }
            }
        }

        public static async Task Profile(HttpContext context, WebApplication app)
        {
            if (context.Session.Keys.Contains("masterId"))
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.SendFileAsync("wwwroot/master/masterProfile.html");
            }
            else context.Response.Redirect("/master/login");
        }
    }
}
