using Microsoft.EntityFrameworkCore;
using HealthentiaVitalsDashboard.Data;
using HealthentiaVitalsDashboard.Services;
using HealthentiaVitalsDashboard.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Register EF Core with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

builder.Services.AddSignalR();

// Register MVC
builder.Services.AddControllersWithViews();
builder.Services.AddHostedService<VitalSignSimulator>();
// Build app
var app = builder.Build();

// Run database seeding
DatabaseSeeder.Seed(app.Services);

// Middleware pipeline
if (!app.Environment.IsDevelopment())
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

app.MapHub<VitalSignsHub>("/vitalsHub");

app.Run();