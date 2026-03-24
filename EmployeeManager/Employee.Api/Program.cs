using Employee.Api.Model;
using Microsoft.EntityFrameworkCore;

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
            .WithOrigins("http://localhost:4200") // Angular app URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // allowed now ✅
    });
});

// Database
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// ================== APP ==================

var app = builder.Build();

// ✅ IMPORTANT ORDER
app.UseCors("AllowFrontend");

// app.UseHttpsRedirection();

app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run
app.Run();