FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# List files so we can see what Docker actually copied
RUN find . -name "*.csproj" -o -name "*.sln"

RUN dotnet restore --verbosity detailed
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TinyUrlApp.dll"]
