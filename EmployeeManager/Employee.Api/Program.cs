using Employee.Api.Model;
using Microsoft.EntityFrameworkCore;
using Employee.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ================== SERVICES ==================

// Controllers
builder.Services.AddControllers();

// ✅ CORS (FIXED PROPERLY)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",                 // local
                "https://workforcemanager.vercel.app"    // deployed frontend ✅
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Database
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<MessageCleanupService>();

builder.Services.AddSignalR();

// ================== APP ==================

var app = builder.Build();

// ✅ IMPORTANT ORDER
app.UseCors("AllowFrontend");

// app.UseHttpsRedirection();

app.UseAuthorization();

// Map controllers
app.MapControllers();

app.UseStaticFiles();

app.MapHub<ChatHub>("/chatHub");

// Run
app.Run();