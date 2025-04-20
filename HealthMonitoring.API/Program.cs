
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using HealthMonitoring.BLL.AutoMapper;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.BLL.Services;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;
using HealthMonitoring.DAL.Repository;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using HealthMonitoring.BLL.StaticData;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HealthMonitoringContext>(options =>
{
    options.UseSqlServer( builder.Configuration.GetConnectionString("Cs"));
   
});
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IMailingService, MailingService>();
builder.Services.AddScoped<ISMSService, SMSService>();
builder.Services.AddAutoMapper(typeof(MppingProfille));
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IActivityDataServices, ActivityDataServices>();
builder.Services.AddScoped<ISensorDataService, SensorDataService>();
builder.Services.AddScoped<IBloodPressurePredictionService, BloodPressurePredictionService>();
builder.Services.AddHttpClient<IBloodPressurePredictionService, BloodPressurePredictionService>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IBaseSrvice, BaseService>(); 
builder.Services.AddHttpClient<BloodPressurePredictionService>();
builder.Services.AddScoped<IEmergencyContactService, EmergencyContactService>();
builder.Services.AddScoped<ISMSService, SMSService>();

builder.Services.AddMemoryCache();

#region Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Options => 
{
    Options.Password.RequireNonAlphanumeric = false;
    Options.Password.RequireLowercase = false;
    Options.Password.RequireUppercase = false;
    Options.Password.RequiredLength = 5;
    Options.SignIn.RequireConfirmedAccount = true;
    Options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;

}) .AddEntityFrameworkStores<HealthMonitoringContext>()
.AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
    opt.TokenLifespan = TimeSpan.FromHours(24)); // default is 1 day
#endregion
#region Authenyication
var key = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
builder.Services.AddAuthentication(a =>
{
    a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };


});
#endregion
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HealthMonitoring v1",
        Version = "v1.0",
        Description = "Api to HealthMonitoring",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Contact Me",
            Url = new Uri("https://example.com/terms"),
        },
        License = new OpenApiLicense
        {
            Name = "Exaple License",
            Url = new Uri("https://example.com/terms")
        }

    });
    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "HealthMonitoring v2",
        Version = "v2.0",
        Description = "Api to HealthMonitoring version 2",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Contact Me",
            Url = new Uri("https://example.com/terms"),
        },
        License = new OpenApiLicense
        {
            Name = "Exaple License",
            Url = new Uri("https://example.com/terms")
        }

    });
    c.UseInlineDefinitionsForEnums();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: `Bearer abc123`"
    });
    // Add a global security requirement
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
                //Scheme = "oauth2",
                //         Name= "Bearer",
                //         In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() ||app.Environment.IsProduction())
{


    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthMonitoring_V1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "HealthMonitoring_V2");
   
        // options.RoutePrefix = string.Empty; // for deploying "/"


    });
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
