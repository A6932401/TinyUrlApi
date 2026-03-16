# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY */TinyUrlApp/TinyUrlApp.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish "/TinyUrlApp/TinyUrlApp.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TinyUrlApp.dll"]
