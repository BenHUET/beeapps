using System.Text;
using BeeApps.Common.Contexts;
using BeeApps.Common.Repositories;
using BeeApps.Common.Services;
using BeeApps.Common.WebAPI.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DB Services
var connectionString =
    $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Database={Environment.GetEnvironmentVariable("DB_NAME")};Username={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
builder.Services.AddDbContext<UserContext>(
    o => o.UseNpgsql(connectionString)
);

// Custom Services
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IHashService, HashService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IMailService, MailService>();

builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddHttpClient();
builder.Services.Configure<TokenOptions>(x =>
{
    x.Key = Environment.GetEnvironmentVariable("JWT_SECRET");
    x.AccessLifetimeInMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_ACCESS_LIFETIME"));
    x.RefreshLifetimeInMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_LIFETIME"));
});
builder.Services.Configure<TokenValidationParameters>(x =>
{
    x.ValidateIssuerSigningKey = true;
    x.IssuerSigningKey =
        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")));
    x.ValidateIssuer = false;
    x.ValidateAudience = false;
    x.ClockSkew = TimeSpan.Zero;
});
builder.Services.Configure<AuthOptions>(x =>
{
    x.ValidateTokenURL = Environment.GetEnvironmentVariable("AUTH_VALIDATE_TOKEN");
    x.ValidateEmailURL = Environment.GetEnvironmentVariable("AUTH_VALIDATE_EMAIL");
    x.RequiresValidation = bool.Parse(Environment.GetEnvironmentVariable("AUTH_REQUIRES_VALIDATION"));
});
builder.Services.Configure<MailOptions>(x =>
{
    x.Host = Environment.GetEnvironmentVariable("MAIl_HOST");
    x.Port = int.Parse(Environment.GetEnvironmentVariable("MAIl_PORT"));
    x.UseSSL = bool.Parse(Environment.GetEnvironmentVariable("MAIl_USE_SSL"));
    x.FromAddress = Environment.GetEnvironmentVariable("MAIl_FROM_ADDRESS");
    x.FromName = Environment.GetEnvironmentVariable("MAIl_FROM_NAME");
    x.Username = Environment.GetEnvironmentVariable("MAIl_USERNAME");
    x.Password = Environment.GetEnvironmentVariable("MAIl_PASSWORD");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserContext>();

    // if (app.Environment.IsDevelopment())
    // context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

app.MapControllers();
app.Run();