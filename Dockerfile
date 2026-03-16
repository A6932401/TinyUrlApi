FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore TinyUrlApp/TinyUrlApp/TinyUrlApp.csproj
RUN dotnet publish TinyUrlApp/TinyUrlApp/TinyUrlApp.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TinyUrlApp.dll"]
