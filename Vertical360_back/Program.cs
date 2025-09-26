using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vertical360_back.Data;
using Vertical360_back.Services;

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

// Configuración de Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Agrega los controladores y las vistas
builder.Services.AddControllersWithViews();

// Registra los servicios personalizados de tu aplicación
builder.Services.AddScoped<IServiceChangeTenant, ServiceChangeTenant>();
builder.Services.AddTransient<IServiceUser, ServiceUser>();

var app = builder.Build();

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