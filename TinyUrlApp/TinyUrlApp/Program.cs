using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Serilog;
using SQLitePCL;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata;
using TinyUrlApp.DAL;
using TinyUrlApp.DAL.Interface;
using TinyUrlApp.EndPoints;
using TinyUrlApp.Handler;
using TinyUrlApp.Logic;
using TinyUrlApp.Logic.Interface;
using TinyUrlApp.Model;
using TinyUrlApp.UnitOfWork;
using TinyUrlApp.Utility;
using TinyUrlApp.validatorValidator;


var builder = WebApplication.CreateBuilder(args);

var appConfigConn = builder.Configuration["Azure:AppConfigConnection"];
var envName = builder.Environment.EnvironmentName;

//Config values taken from Azure
builder.Configuration.AddAzureAppConfiguration(option =>
{
    option.Connect(appConfigConn)
    .Select("*", envName);
});

// Config values assign to AppSetting
builder.Services
    .Configure<AppSetting>(builder.Configuration.GetSection(AppSetting.SECNAME));

var appSettings = builder.Configuration
    .GetSection(AppSetting.SECNAME)
    .Get<AppSetting>();

// Cors Config
builder.Services.AddCors(options =>
{
    options.AddPolicy("TinyUrlApp",
        policy =>
        {
            policy.WithOrigins(appSettings.CorsOrigins) 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

//Config Serilog
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.AzureBlobStorage(
        connectionString: appSettings.blobStorageConnection, 
        storageContainerName: appSettings.blobStorageContainer,  
       storageFileName: appSettings.blobStorageFileFormat,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
    )

                .WriteTo.File("Logs/Logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
builder.Host.UseSerilog();
// Add services to the container.


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<EndPointValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EndPointIdValidator>();

builder.Services.AddScoped<IEndPointLogic, EndPointLogic>(); 
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEndPointDA, EndPointDA>();
builder.Services.AddScoped<DbConnection>(sp =>
{
    var connection = new SqlConnection(appSettings.dbConnection);
    
    connection.Open(); 
    return connection;
});


var app = builder.Build();
app.UseCors("TinyUrlApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Middleware 
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Create a group with /api prefix
var appApi = app.MapGroup("/api");

// Register endpoints under /api
appApi.LinkEndpoints();


app.Run();
