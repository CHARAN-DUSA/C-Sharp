using Employee.Api.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// ✅ CORS (STRICT + CORRECT)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://workforcemanager.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // important
    });
});

// DB
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ✅ VERY IMPORTANT ORDER
app.UseCors("AllowFrontend");   // MUST be before everything

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();