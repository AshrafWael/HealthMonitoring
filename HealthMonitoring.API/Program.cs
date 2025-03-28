
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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HealthMonitoringContext>(options =>
{
    options.UseSqlServer( builder.Configuration.GetConnectionString("Cs"));
   
});
builder.Services.AddAutoMapper(typeof(MppingProfille));


builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IActivityDataServices, ActivityDataServices>();
builder.Services.AddScoped<ISensorDataService, SensorDataService>();
builder.Services.AddScoped<IBloodPressurePredictionService, BloodPressurePredictionService>();
builder.Services.AddHttpClient<IBloodPressurePredictionService, BloodPressurePredictionService>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IBaseSrvice, BaseService>(); 
builder.Services.AddHttpClient<BloodPressurePredictionService>();
builder.Services.AddMemoryCache();

#region Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Options => 
{
    Options.Password.RequireNonAlphanumeric = false;
    Options.Password.RequireLowercase = false;
    Options.Password.RequireUppercase = false;
    Options.Password.RequiredLength = 5;

}) .AddEntityFrameworkStores<HealthMonitoringContext>();
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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
