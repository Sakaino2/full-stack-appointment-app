using Application.Services;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
    ));

builder.Services.AddControllers();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddScoped<IGoogleCalendarService, MockGoogleCalendarService>();

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();
        await DatabaseSeeder.SeedAdminUserAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();