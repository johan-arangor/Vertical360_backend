using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Principal;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Application.UseCases.Implementations;
using Vertical360_back.Infrastructure.Persistence;
using Vertical360_back.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// Obtiene la cadena de conexión de appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra los servicios para la detección y gestión del inquilino
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IServiceTenant, ServiceTenant>();

// Configura Entity Framework con MySQL
builder.Services.AddDbContext<ApplicationDbContext>((options) =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Configuración de Identity con roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Agrega los controladores y Api's
builder.Services.AddControllers();

// Registra los servicios personalizados de tu aplicación
builder.Services.AddScoped<IServiceChangeTenant, ServiceChangeTenant>();
builder.Services.AddTransient<IServiceUser, ServiceUser>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configura el middleware de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Ejecutar el seeder antes de levantar la app
await IdentityDataSeeder.SeedSuperAdminAsync(app.Services);

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();