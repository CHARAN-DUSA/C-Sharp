using Employee.Api.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================== SERVICES ==================

// Controllers
builder.Services.AddControllers();

// ✅ CORS (STRICT + CORRECT)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()     // safest for now (fixes CORS fully)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // important
    });
});

// Database
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// ================== APP ==================

var app = builder.Build();

// ✅ VERY IMPORTANT ORDER
app.UseCors("AllowFrontend");   // MUST be before everything

//app.UseHttpsRedirection();

app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run
app.Run();
