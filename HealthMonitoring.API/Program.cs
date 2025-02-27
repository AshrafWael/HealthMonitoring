
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


//builder.Services.AddScoped(typeof(IBaseRepository<>),typeof(BaseRepository<>));
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IActivityDataServices, ActivityDataServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
#region Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<HealthMonitoringContext>();
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
