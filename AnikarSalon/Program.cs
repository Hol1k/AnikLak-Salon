var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/", async (context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/index.html");
});

app.MapGet("/login", async (context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/signup.html");
});

app.Run();
