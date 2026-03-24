using Employee.Api.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================== SERVICES ==================

// Controllers
builder.Services.AddControllers();

// ✅ CORS (robust + preflight safe)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()     // safest for now (fixes CORS fully)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Database
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// ================== APP ==================

var app = builder.Build();

// ✅ ORDER IS CRITICAL
app.UseCors("AllowFrontend");   // must be BEFORE MapControllers

//app.UseHttpsRedirection();

app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run
app.Run();
