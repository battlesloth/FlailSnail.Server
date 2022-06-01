using System.Text;
using FlailSnail.Server.Configuration;
using FlailSnail.Server.Database;
using FlailSnail.Server.Security;
using FlailSnail.Server.Utility.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(LoggingSwitches.LogLevelSwitch)
    .Enrich.FromLogContext()
    .CreateLogger();

#if DEBUG
LoggingSwitches.LogLevelSwitch.MinimumLevel = LogEventLevel.Debug;
#else 
LoggingSwitches.LogLevelSwitch.MinimumLevel = LogEventLevel.Information;
#endif

Log.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Configuration.AddJsonFile("appsettings.json");

#if DEBUG
builder.Configuration.AddJsonFile("appsettings.Development.json");
#endif

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var jwtSecurity = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put **_ONLY_** your JWT token in the textbox below",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.SwaggerDoc("v1", new OpenApiInfo {Title = "Flail Snail", Version = "v1"});
    c.AddSecurityDefinition(jwtSecurity.Reference.Id, jwtSecurity);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {jwtSecurity, Array.Empty<string>()}
    });
});

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
            ValidateIssuer = true,
            ValidateAudience = true,
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

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<SlidingExpirationMiddleware>();

app.MapControllers();

app.Run();
