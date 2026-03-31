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
var configuration = builder.Configuration;

// ─── 1. Master DB (Identity + Companies + datos compartidos) ────────────────
var masterConn = configuration.GetConnectionString("SharedDb")
    ?? throw new InvalidOperationException("ConnectionString 'SharedDb' no encontrada.");

builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseMySql(masterConn, ServerVersion.AutoDetect(masterConn),
        mySql => mySql.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null)));

// ─── 2. Identity sobre MasterDbContext ──────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
})
.AddEntityFrameworkStores<MasterDbContext>()
.AddDefaultTokenProviders();

// ─── 3. JWT ──────────────────────────────────────────────────────────────────
var jwtSection = configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);

var jwtKey = jwtSection.GetValue<string>("Key")
    ?? throw new InvalidOperationException("JwtSettings:Key no configurada.");

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
        ValidIssuer = jwtSection.GetValue<string>("Issuer"),
        ValidAudience = jwtSection.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// ─── 4. CORS ─────────────────────────────────────────────────────────────────
var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontPolicy", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ─── 5. Servicios de aplicación ──────────────────────────────────────────────
builder.Services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

// Infraestructura compartida
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ApplicationDbSeeder>();

// Tenant resolution
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
builder.Services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
builder.Services.AddScoped<ITenantDatabaseService, TenantDatabaseService>();

// Auth
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

// Repositorios master
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Repositorios tenant (usan ITenantDbContextFactory internamente)
builder.Services.AddScoped<IResidentRepository, ResidentRepository>();

// Servicios de dominio
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IResidentService, ResidentService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title       = "Vertical360 API",
        Version     = "v1",
        Description = "API REST para la administración de propiedades horizontales. " +
                      "Gestiona unidades residenciales (tenants), residentes, visitantes, " +
                      "domicilios, pagos y comunicación interna.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name  = "Equipo Vertical360",
            Email = "soporte@vertical360.com"
        }
    });

    // Esquema de seguridad JWT — aparece el botón "Authorize" en Swagger UI
    var jwtScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Ingresa el token JWT obtenido en **/api/auth/select-tenant**.\n\n" +
                       "Formato: `Bearer {token}`"
    };
    var jwtRef = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Id   = "Bearer",
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    };
    options.AddSecurityDefinition("Bearer", jwtScheme);
    options.AddSecurityRequirement(jwtRef);

    // Habilita [SwaggerOperation], [SwaggerResponse] y demás anotaciones
    options.EnableAnnotations();

    // Ordena los endpoints por nombre del controller (tag)
    options.OrderActionsBy(api => $"{api.ActionDescriptor.RouteValues["controller"]}_{api.HttpMethod}");

    // Incluye comentarios XML si el archivo existe
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// ─── 6. Pipeline ─────────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Vertical360 API v1");
        options.RoutePrefix         = "swagger";
        options.DocumentTitle       = "Vertical360 — API Docs";
        options.DefaultModelsExpandDepth(1);
        options.DefaultModelExpandDepth(2);
        options.DisplayRequestDuration();
        options.EnableFilter();
        options.EnableDeepLinking();
        // Persiste el token JWT entre recargas de la página
        options.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
}

app.UseCors("FrontPolicy");
app.UseRouting();
app.UseAuthentication();

// TenantMiddleware DESPUÉS de UseAuthentication para que el JWT ya esté procesado
app.UseMiddleware<TenantMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ─── 7. Seed inicial ─────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>();
    await seeder.SeedAsync();
}

app.Run();
