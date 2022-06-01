using System.Text;
using FlailSnail.Server.Configuration;
using FlailSnail.Server.Database;
using FlailSnail.Server.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Configuration.AddJsonFile("appsettings.json");

#if DEBUG
builder.Configuration.AddJsonFile("appsettings.Development.json");
#endif

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(nameof(DatabaseOptions)));

builder.Services.AddScoped<IUserRepository, NpgUserRepository>();

var secret = builder.Configuration.GetSection(nameof(ServiceOptions))[nameof(ServiceOptions.JwtSecret)];
var issuer = builder.Configuration.GetSection(nameof(ServiceOptions))[nameof(ServiceOptions.JwtIssuer)];

builder.Services.AddTransient<ITokenService, JwtTokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = issuer,
            ValidAudience = issuer,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))

        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseMiddleware<SlidingExpirationMiddleware>();

app.MapControllers();

app.Run();
