using Employee.Api.Model;
using Microsoft.EntityFrameworkCore;
using Employee.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ================== SERVICES ==================

// Controllers
builder.Services.AddControllers();

// ✅ CORS (FINAL WORKING VERSION)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost") ||   // local
                origin.Contains("vercel.app")              // all Vercel deployments
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // required for SignalR
    });
});

// Database
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Background service
builder.Services.AddHostedService<MessageCleanupService>();

// SignalR
builder.Services.AddSignalR();

// ================== APP ==================

var app = builder.Build();

// ✅ VERY IMPORTANT ORDER

// CORS MUST be first
app.UseCors("AllowFrontend");

// Optional HTTPS (Render handles it, so safe to skip)
// app.UseHttpsRedirection();

// Static files (optional)
app.UseStaticFiles();

// Authorization (if you add auth later)
app.UseAuthorization();

// Map controllers
app.MapControllers();

// SignalR Hub
app.MapHub<ChatHub>("/chatHub");

// Run app
app.Run();