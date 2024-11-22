using BeeApps.Common.Services;
using BeeApps.Common.WebAPI.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Custom Services
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddHttpClient();
builder.Services.Configure<AuthOptions>(x =>
{
    x.ValidateTokenURL = Environment.GetEnvironmentVariable("AUTH_VALIDATE_TOKEN");
});

var app = builder.Build();

app.MapControllers();
app.Run();