using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MPCalcHub.Api.Logging;
using MPCalcHub.Domain.Interfaces;
using MPCalcHub.Domain.Interfaces.Infrastructure;
using MPCalcHub.Domain.Services;
using MPCalcHub.Infrastructure.Data;
using MPCalcHub.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Reflection;
using MPCalcHub.Application.Interfaces;
using MPCalcHub.Application.Services;
using MPCalcHub.Domain.Enums;
using MPCalcHub.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using static MPCalcHub.Domain.Constants.AppConstants;
using MPCalcHub.Application.DataTransferObjects;
using MPCalcHub.Infrastructure.Security;
using MPCalcHub.Domain.Interfaces.Security;
using MPCalcHub.Domain.Services.Security;
using MPCalcHub.Domain.Settings;
using MPCalcHub.Domain.Entities;
using MPCalcHub.Api.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Prometheus;
using MPCalcHub.Api.Robots.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var jwtKeyConfig = builder.Configuration["Token:Key"];
if (string.IsNullOrEmpty(jwtKeyConfig))
    throw new InvalidOperationException("Token:Key configuration is missing or empty.");

builder.Services.Configure<MPCalcHubSettings>(builder.Configuration);
builder.Services.AddHealthChecks().ForwardToPrometheus();
builder.WebHost.UseUrls("https://0.0.0.0:5056");

builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtKeyConfig)),
        RequireExpirationTime = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicyWithPermission(Policies.SuperUser, PermissionLevel.SuperUser)
           .AddPolicyWithPermission(Policies.SuperOrModerator, PermissionLevel.SuperUser | PermissionLevel.Moderator)
           .AddPolicyWithPermission(Policies.User, PermissionLevel.User)
           .AddPolicyWithPermission(Policies.Guest, PermissionLevel.Guest)
           .AddPolicyWithPermission(Policies.Banned, PermissionLevel.Banned)
           .AddPolicyWithPermission(Policies.SuperOrGuest, PermissionLevel.SuperUser | PermissionLevel.Guest);
}).AddAuthorizationBuilder();

builder.Services.AddControllers(options => options.Filters.Add<UserFilter>()).AddNewtonsoftJson(options =>
{
    var settings = options.SerializerSettings;
    settings.NullValueHandling = NullValueHandling.Ignore;
    settings.FloatFormatHandling = FloatFormatHandling.DefaultValue;
    settings.FloatParseHandling = FloatParseHandling.Double;
    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    settings.DateFormatString = "yyyy-MM-ddTHH:mm:ss";
    settings.Culture = new CultureInfo("en-US");
    settings.Converters.Add(new StringEnumConverter());
    settings.ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MPCalcHub API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.CustomSchemaIds(type => 
    {
        var namingStrategy = new SnakeCaseNamingStrategy();
        return namingStrategy.GetPropertyName(type.Name, false);
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization Header - utilizado com Bearer Authentication. \r\n\r\n Insira 'Bearer' [espaço] e então seu token na caixa abaixo.\r\n\r\nExemplo: (informar sem as aspas): 'Bearer 1234sdfgsdf' ",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAutoMapper((sp, cfg) =>
{
    cfg.AllowNullDestinationValues = true;
    cfg.AllowNullCollections = true;
    cfg.ConstructServicesUsing(sp.GetService);
}, Assembly.GetAssembly(typeof(BaseModel)));

builder.Logging.ClearProviders();
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection"));
    options.LogTo(message => Debug.WriteLine(message), LogLevel.Information);
    options.EnableSensitiveDataLogging();
});

builder.Services.AddMemoryCache();

// Inject all Services and repositories

#region Repositories

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IStateDDDRepository, StateDDDRepository>();

#endregion

#region Services

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IStateDDDService, StateDDDService>();

#endregion

#region Application Services

builder.Services.AddScoped<IUserApplicationService, UserApplicationService>();
builder.Services.AddScoped<ITokenApplicationService, TokenApplicationService>();
builder.Services.AddScoped<IContactApplicationService, ContactApplicationService>();

#endregion

#region Authorization

builder.Services.AddSingleton<IAuthorizationHandler, RolesAuthorizationHandler>();

#endregion

#region Filters

builder.Services.AddScoped<IAuthorizationFilter, UserFilter>();
builder.Services.AddScoped(x => new UserData());

#endregion

#region RabbitMQConsumers

builder.Services.AddHostedService<ContactConsumerService>();

#endregion

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MPCalcHub API 2025"));

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDBContext>();

    context.Database.Migrate();
}


app.UseHealthChecks("/health");
app.UseHttpMetrics();
app.MapMetrics();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
