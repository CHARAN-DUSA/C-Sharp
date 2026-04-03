# ─────────────────────────────────────────────────
# STAGE 1: Build the .NET 8 API
# ─────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy project file first for better caching
# dotnet restore won't re-run unless .csproj changes
COPY Employee.Api.csproj .

# Restore all NuGet packages
RUN dotnet restore

# Now copy all remaining source files
COPY . .

# Publish in Release mode to /app/out
RUN dotnet publish -c Release -o out

# ─────────────────────────────────────────────────
# STAGE 2: Run the .NET 8 API
# Lightweight runtime image (no SDK — smaller size)
# ─────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/out .

# Expose port 8080 (standard for containers)
EXPOSE 8080

# Set the URL the API listens on inside the container
ENV ASPNETCORE_URLS=http://+:8080

# Start the API
ENTRYPOINT ["dotnet", "Employee.Api.dll"]