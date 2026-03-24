using Employee.Api.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================== SERVICES ==================

// Controllers
builder.Services.AddControllers();

// ✅ CORS (for Vercel frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://workforcemanager.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Database
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// ================== APP ==================

var app = builder.Build();

// ✅ CORS MUST be before routing
app.UseCors("AllowFrontend");

// Optional
// app.UseHttpsRedirection();

app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run app
app.Run();
