using BeeApps.Common.Services;
using BeeApps.Common.WebAPI.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Custom Services
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Authentication"));
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapControllers();
app.Run();