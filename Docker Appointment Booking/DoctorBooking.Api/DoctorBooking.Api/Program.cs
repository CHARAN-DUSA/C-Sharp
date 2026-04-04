using AspNetCoreRateLimit;
using DoctorBooking.API.API.Hubs;
using DoctorBooking.API.API.Middleware;
using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Application.Validators;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Infrastructure.Data;
using DoctorBooking.API.Infrastructure.Repositories;
using DoctorBooking.API.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// ── DATABASE ────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o =>
        {
            o.CommandTimeout(120);
            o.EnableRetryOnFailure();
        }
    )
);

// ── IDENTITY ────────────────────────────────────────────────
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── JWT AUTH ────────────────────────────────────────────────
var jwt = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Secret"]!)
        )
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken) &&
                context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ── RATE LIMITING ───────────────────────────────────────────
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(
    builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(
    builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ── VALIDATION ──────────────────────────────────────────────
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<BookAppointmentValidator>();

// ── REPOSITORIES ────────────────────────────────────────────
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// ── SERVICES ────────────────────────────────────────────────
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.AddHostedService<AppointmentCleanupService>();
builder.Services.AddScoped<TwoFactorService>();
builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddSingleton<FirebaseAdminService>();

// ── SIGNALR ─────────────────────────────────────────────────
builder.Services.AddSignalR();

// ── CORS (FIXED) ────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercel", policy =>
    {
        policy
            .WithOrigins(
                "https://medibook-2ollzo2o4-charans-projects-033f5765.vercel.app",
                "https://medibook-five-rosy.vercel.app/",
                "https://*.vercel.app",   // covers all preview deployments
                "http://localhost:4200"   // for local dev
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // only if you use cookies/auth headers
    });
});

// ── CONTROLLERS + SWAGGER ───────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MediBook API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CONFIG");

if (!string.IsNullOrEmpty(firebaseJson) && FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromJson(firebaseJson)
    });
}

var app = builder.Build();
// ── PIPELINE (FIXED ORDER) ──────────────────────────────────

// 1. Rate limiting + exception handling
app.UseIpRateLimiting();
app.UseMiddleware<ExceptionMiddleware>();

// 2. Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ❌ IMPORTANT: DO NOT USE THIS (causes your error)
// app.UseHttpsRedirection();

// 3. CORS MUST BE BEFORE AUTH
app.UseCors("AllowVercel");

// 4. Authentication
app.UseAuthentication();
app.UseAuthorization();

// 5. Endpoints
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

// ── DATABASE SEEDING ────────────────────────────────────────
_ = Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(scope.ServiceProvider);
});
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();

public partial class Program { }