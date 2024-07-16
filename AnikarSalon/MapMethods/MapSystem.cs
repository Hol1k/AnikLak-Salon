using AnikarSalon.DataPersistence.PostgreSQL;
using AnikarSalon.DataPersistence.PostgreSQL.Repositories;
using AnikarSalon.Persistence.Postgres.Models;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AnikarSalon.MapMethods
{
    public static class MapSystem
    {
        public static async Task GetMastersList(HttpContext context, WebApplication app)
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
        }

        public static async Task CheckFreeRegistrationTimes(HttpContext context, WebApplication app)
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

            await context.Response.WriteAsync(string.Join(",", freeRegistrationTimes.ToArray()));
        }

        public static async Task GetMasterName(HttpContext context, WebApplication app)
        {
            if (context.Session.Keys.Contains("masterName"))
            {
                string userName = context.Session.GetString("masterName") ?? "Профиль";
                await context.Response.WriteAsync(userName);
            }
            else await context.Response.WriteAsync("");
        }

        public static async Task GetUserName(HttpContext context, WebApplication app)
        {
            if (context.Session.Keys.Contains("username"))
            {
                string userName = context.Session.GetString("username") ?? "Профиль";
                await context.Response.WriteAsync(userName);
            }
            else await context.Response.WriteAsync("");
        }

        public static async Task GetServicePrice(HttpContext context, WebApplication app)
        {
            decimal price;
            (_, price) = AppointmentData.Get(context.Request.Form["service"].ToString());
            await context.Response.WriteAsync(price.ToString());
        }

        public static async Task CheckLogin(HttpContext context, WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                ClientsRepository clientsRepo;
                var services = scope.ServiceProvider;
                clientsRepo = services.GetRequiredService<ClientsRepository>();
                var loginData = context.Request.Form;

                ClientEntity? user = await clientsRepo.Login(
                    phoneNumber: AdditionalMethods.PhoneBuilder(loginData["userPhone"].ToString()),
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
        }

        public static async Task CheckMasterLogin(HttpContext context, WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                MastersRepository mastersRepo;
                var services = scope.ServiceProvider;
                mastersRepo = services.GetRequiredService<MastersRepository>();
                var loginData = context.Request.Form;

                MasterEntity? user = await mastersRepo.Login(
                    phoneNumber: AdditionalMethods.PhoneBuilder(loginData["userPhone"].ToString()),
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
        }

        public static async Task CheckClientAppointments(HttpContext context, WebApplication app)
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

                    if (appointmentsList == null) await context.Response.WriteAsync($"{{\"name\":\"{context.Session.GetString("username")}\",\"appointments\":[" + "]}");
                    else
                    {
                        StringBuilder appointmentsJsonBuild = new StringBuilder($"{{\"name\":\"{context.Session.GetString("username")}\",\"appointments\":[");
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
            }
        }

        public static async Task CheckMasterAppointments(HttpContext context, WebApplication app)
        {
            if (context.Session.Keys.Contains("masterId"))
            {
                using (var scope = app.Services.CreateScope())
                {
                    MastersRepository mastersRepo;
                    var services = scope.ServiceProvider;
                    mastersRepo = services.GetRequiredService<MastersRepository>();
                    var masterId = context.Session.GetString("masterId");

                    var appointmentsList = await mastersRepo.GetAllAppointments(masterId ?? "");

                    if (appointmentsList == null) await context.Response.WriteAsync("{\"appointments\":[" + "]}");
                    else
                    {
                        StringBuilder appointmentsJsonBuild = new StringBuilder("{\"appointments\":[");
                        foreach (var appointment in appointmentsList)
                        {
                            string date = appointment.DateTime.Year.ToString() + '-'
                            + (appointment.DateTime.Month < 10 ? '0' + appointment.DateTime.Month.ToString() : appointment.DateTime.Month) + '-'
                            + (appointment.DateTime.Day < 10 ? '0' + appointment.DateTime.Day.ToString() : appointment.DateTime.Day) + ' '
                            + appointment.DateTime.Hour.ToString() + ':'
                            + (appointment.DateTime.Minute < 10 ? '0' + appointment.DateTime.Minute.ToString() : appointment.DateTime.Minute);

                            string number = appointment.Client.PhoneNumber;
                            number = "+7" + number.Insert(9, "-").Insert(7, "-").Insert(4, ") ").Insert(1, " (").Remove(0, 1);

                            string appointmentJson = $"{{\"appointmentId\":\"{appointment.Id}\"," +
                            $"\"client\":\"{appointment.Client.FirstName}\"," +
                            $"\"number\":\"{number}\"," +
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
            }
        } 

        public static async Task IsNumberOccupied(HttpContext context, WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                ClientsRepository clientsRepo;
                var services = scope.ServiceProvider;
                clientsRepo = services.GetRequiredService<ClientsRepository>();
                var signupData = context.Request.Form;

                string phoneNumber = AdditionalMethods.PhoneBuilder(signupData["userPhone"].ToString());

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
        }

        public static async Task GetMasterInfo(HttpContext context, WebApplication app)
        {
            string masterId = "";
            if (context.Session.Keys.Contains("masterId")) masterId = context.Session.GetString("masterId") ?? "";
            if (context.Request.Query.Keys.Contains("masterId")) { masterId = context.Request.Query["masterId"].ToString(); }
            if (masterId == "")
            {
                await context.Response.WriteAsync("");
                return;
            }

            using (var scope = app.Services.CreateScope())
            {
                MastersRepository mastersRepo;
                var services = scope.ServiceProvider;
                mastersRepo = services.GetRequiredService<MastersRepository>();

                var master = await mastersRepo.GetById(masterId);

                string masterJson = $"{{\"name\":\"{master.FirstName}\"," +
                $"\"lastname\":\"{master.LastName}\"," +
                $"\"exp\":\"{master.YearsExperience}\"," +
                $"\"service\":\"{master.Services[0]}\"," +
                $"\"about\":\"{master.About}\"," +
                $"\"avatar\":\"{master.AvatarUrl}\"}}";

                await context.Response.WriteAsync(masterJson);
            }
        }

        public static async Task UpdateMasterInfo(HttpContext context, WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                MastersRepository mastersRepo;
                var services = scope.ServiceProvider;
                mastersRepo = services.GetRequiredService<MastersRepository>();
                var newAbout = context.Request.Form["about"].ToString();

                await mastersRepo.UpdateAbout(context.Session.GetString("masterId").ToString(), newAbout);
            }
        }

        public static async Task WriteComment(HttpContext context, WebApplication app)
        {
            string userId = context.Session.GetString("userId") ?? "";
            string masterId = context.Request.Query["masterId"].ToString();
            string commentText = context.Request.Query["commentText"].ToString();

            using (var scope = app.Services.CreateScope())
            {
                CommentsRepository comentsRepo;
                var services = scope.ServiceProvider;
                comentsRepo = services.GetRequiredService<CommentsRepository>();

                await comentsRepo.WriteComment(Guid.Parse(masterId), Guid.Parse(userId), commentText);
            }

            context.Response.Redirect("/masters?masterId=" + masterId);
        }

        public static async Task GetComments(HttpContext context, WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                CommentsRepository comentsRepo;
                var services = scope.ServiceProvider;
                comentsRepo = services.GetRequiredService<CommentsRepository>();

                string masterId = "";
                if (context.Session.Keys.Contains("masterId")) masterId = context.Session.GetString("masterId") ?? "";
                if (context.Request.Query.Keys.Contains("masterId")) masterId = context.Request.Query["masterId"].ToString();

                var commentsList = await comentsRepo.GetComments(Guid.Parse(masterId));

                if (commentsList == null) await context.Response.WriteAsync("{\"comments\":[" + "]}");
                else
                {
                    StringBuilder commentsJsonBuild = new StringBuilder("{\"comments\":[");
                    foreach (var comment in commentsList)
                    {
                        string date = comment.DateTime.Year.ToString() + '-'
                        + (comment.DateTime.Month < 10 ? '0' + comment.DateTime.Month.ToString() : comment.DateTime.Month) + '-'
                        + (comment.DateTime.Day < 10 ? '0' + comment.DateTime.Day.ToString() : comment.DateTime.Day) + ' '
                        + comment.DateTime.Hour.ToString() + ':'
                        + (comment.DateTime.Minute < 10 ? '0' + comment.DateTime.Minute.ToString() : comment.DateTime.Minute);

                        string commentJson = $"{{\"commentId\":\"{comment.Id}\"," +
                        $"\"author\":\"{comment.Author.FirstName}\"," +
                        $"\"date\":\"{date}\"," +
                        $"\"comment\":\"{comment.Text}\"}},";
                        commentsJsonBuild.Append(commentJson);
                    }
                    if (commentsList.Count > 0) commentsJsonBuild.Remove(commentsJsonBuild.Length - 1, 1);
                    commentsJsonBuild.Append("]}");

                    await context.Response.WriteAsync(commentsJsonBuild.ToString());
                }
            }
        }

        public static async Task IsClientWasAtMaster(HttpContext context, WebApplication app)
        {
            string masterId = context.Request.Query["masterId"].ToString();
            string clientId = context.Session.GetString("userId") ?? "";

            using (var scope = app.Services.CreateScope())
            {
                MastersRepository mastersRepo;
                var services = scope.ServiceProvider;
                mastersRepo = services.GetRequiredService<MastersRepository>();
                
                var isClientWas = await mastersRepo.IsClientWas(Guid.Parse(masterId), Guid.Parse(clientId));

                if (isClientWas) await context.Response.WriteAsync("true");
                else await context.Response.WriteAsync("false");
            }
        }
    }
}
