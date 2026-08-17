using Application.Services;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.ExternalServices.GoogleCalendar;
using Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

// ============ CONFIGURACIÓN DE CORS ============
// Agregar CORS antes que cualquier otra configuración
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()      // Permite cualquier origen
              .AllowAnyMethod()      // Permite cualquier método (GET, POST, PUT, DELETE, etc.)
              .AllowAnyHeader();     // Permite cualquier encabezado
    });

    // Opción más segura para desarrollo (recomendada)
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",           // Angular en desarrollo
                "http://localhost:4201",           // Angular en producción local
                "http://host.docker.internal:4200" // Desde Docker
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Permite enviar cookies/tokens
    });
});

// ============ CONFIGURACIÓN DE SERVICIOS ============
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<GoogleCalendarSettings>(
    builder.Configuration.GetSection(GoogleCalendarSettings.SectionName));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
    )
    .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();

var jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? throw new InvalidOperationException("JwtSettings:Secret is missing.");

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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ============ MIGRACIONES Y SEED ============
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
        await DatabaseSeeder.SeedAdminUserAsync(services);
        await DatabaseSeeder.SeedClientAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// ============ MIDDLEWARE ============
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// IMPORTANTE: CORS debe ir ANTES de UseRouting y UseAuthentication
app.UseCors("AllowAngularDev");  // Usar la política específica para Angular

// O si prefieres permitir todo (menos seguro):
// app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();