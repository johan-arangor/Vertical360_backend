using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vertical360_backend.Application.Configuration;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Infrastructure.Auth;
using Vertical360_backend.Infrastructure.Middleware;
using Vertical360_backend.Infrastructure.Persistence;
using Vertical360_backend.Infrastructure.Repositories;
using Vertical360_backend.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Config
var configuration = builder.Configuration;

// DB (Shared)
var sharedConn = configuration.GetConnectionString("SharedDb")
    ?? throw new InvalidOperationException("ConnectionString SharedDb no encontrada.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(sharedConn, ServerVersion.AutoDetect(sharedConn)));

// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT
var jwt = configuration.GetSection("JwtSettings");
var key = jwt.GetValue<string>("Key");

builder.Services.Configure<JwtSettings>(jwt);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt.GetValue<string>("Issuer"),
        ValidAudience = jwt.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

// CORS - solo desde frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontPolicy", policy =>
    {
        policy.WithOrigins(configuration.GetSection("Cors:AllowedOrigins").Get<string[]>())
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// DI - interfaces y servicios
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<ApplicationDbSeeder>();
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
builder.Services.AddScoped<ITenantDatabaseService, TenantDatabaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IResidentService, ResidentService>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IResidentRepository, ResidentRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontPolicy");
app.UseRouting();
app.UseAuthentication();
// Tenant middleware debe ejecutarse antes de autorizar peticiones que dependan del tenant
app.UseMiddleware<TenantMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

// Inicial seed (shared)
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>();
    await seeder.SeedAsync();
}

app.Run();