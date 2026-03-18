using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using GameStore.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRepositories(builder.Configuration);

// Add Authentication and Authorization
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

await app.Services.InitializeDbAsync();

// Use Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapGamesEndpoints();

app.Run();