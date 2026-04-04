# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY *.csproj ./
RUN dotnet restore

# Copy everything and build
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy from build
COPY --from=build /app/out ./

# Expose port
EXPOSE 8080

# Start app
ENTRYPOINT ["dotnet", "DoctorBooking.Api.dll"]