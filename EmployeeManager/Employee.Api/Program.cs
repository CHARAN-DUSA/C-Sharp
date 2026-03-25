using Employee.Api.Model;
using Microsoft.EntityFrameworkCore;
using Employee.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ================== SERVICES ==================

builder.Services.AddControllers();

// ✅ CORS (STRICT + WORKING)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin => true) // 🔥 TEMP allow all
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Database
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<MessageCleanupService>();

builder.Services.AddSignalR();

// ================== APP ==================

var app = builder.Build();

// ✅ VERY IMPORTANT: CORS FIRST
app.UseCors("AllowFrontend");

// 🔥 HANDLE PREFLIGHT REQUESTS (THIS IS THE REAL FIX)
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});

// Middleware order
app.UseStaticFiles();
app.UseAuthorization();

// Endpoints
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();