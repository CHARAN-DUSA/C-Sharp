using Employee.Api.Model;
using Microsoft.EntityFrameworkCore;
using Employee.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ================== SERVICES ==================

// Controllers
builder.Services.AddControllers();

// ✅ CORS (FINAL FIX - supports Vercel dynamic URLs)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost") || 
                origin.Contains("vercel.app")          // ✅ allows all Vercel deployments
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();                      // ✅ needed for SignalR
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

// ✅ ORDER IS VERY IMPORTANT
app.UseCors("AllowFrontend");

// app.UseHttpsRedirection(); // optional (Render handles HTTPS)

app.UseAuthorization();

// Static files (if needed)
app.UseStaticFiles();

// Map controllers
app.MapControllers();

// SignalR Hub
app.MapHub<ChatHub>("/chatHub");

// Run app
app.Run();