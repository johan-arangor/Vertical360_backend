using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vertical360_back.Application.Configuration;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Application.UseCases.Implementations;
using Vertical360_back.Infrastructure.Auth;
using Vertical360_back.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Obtiene la cadena de conexión de (Common y Template) ---
var commonConnectionString = builder.Configuration.GetConnectionString("CommonConnection");
var tenantConnectionStringTemplate = builder.Configuration.GetConnectionString("TenantConnectionTemplate");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Mapear la configuración de JWT desde appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


// Registra los servicios para la detección y gestión del inquilino
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IServiceTenant, ServiceTenant>();

// Configura el CommonDbContext (Conexión Fija) ---
builder.Services.AddDbContext<CommonDbContext>((options) =>
{
    options.UseMySql(commonConnectionString, ServerVersion.AutoDetect(commonConnectionString));
});

// Configura el ApplicationDbContext (Conexión Dinámica) ---
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    // Resuelve el nombre de la BD del cliente en tiempo de solicitud (Request Scope)
    var tenantService = serviceProvider.GetRequiredService<IServiceTenant>();
    var finalConnectionString = tenantService.GetTenantConnectionString();

    if (string.IsNullOrEmpty(finalConnectionString))
    {
        finalConnectionString = builder.Configuration.GetConnectionString("CommonConnection");
    }

    // Construye la cadena de conexión final usando la plantilla
    options.UseMySql(finalConnectionString, ServerVersion.AutoDetect(finalConnectionString));
});

// Configuración de Identity (Usa ApplicationDbContext por defecto)
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configuración de Autenticación JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Cambiar a true en producción
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

// Registra servicio para la administración de usuarios
builder.Services.AddScoped<IAuthService, AuthService>();

// Agrega los controladores y las vistas
builder.Services.AddControllersWithViews();

// Registra los servicios personalizados de tu aplicación
builder.Services.AddScoped<IServiceChangeTenant, ServiceChangeTenant>();
builder.Services.AddTransient<IServiceUser, ServiceUser>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// EJECUTA EL SEEDER AL INICIO
using (var scope = app.Services.CreateScope())
{
    try
    {
        // Obtener los servicios base
        var services = scope.ServiceProvider;
        // Crear el Seeder de las dependencias identity
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        // Crear el Seeder manualmente con las dependencias simplificadas
        var commonContext = services.GetRequiredService<CommonDbContext>();
        var environment = services.GetRequiredService<IWebHostEnvironment>();

        var seeder = new CommonDbSeeder(commonContext, environment, userManager, roleManager);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error durante la inicialización de la Base de Datos Común.");
    }
}

// Configura el middleware de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();