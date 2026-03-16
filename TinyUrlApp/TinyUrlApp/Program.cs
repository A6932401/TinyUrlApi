using FluentValidation;
using Microsoft.Data.Sqlite;
using Serilog;
using SQLitePCL;
using System;
using System.Data;
using System.Data.Common;
using TinyUrlApp.DAL;
using TinyUrlApp.DAL.Interface;
using TinyUrlApp.EndPoints;
using TinyUrlApp.Handler;
using TinyUrlApp.Logic;
using TinyUrlApp.Logic.Interface;
using TinyUrlApp.Model;
using TinyUrlApp.UnitOfWork;

using TinyUrlApp.validatorValidator;
var builder = WebApplication.CreateBuilder(args);

//test
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("TinyUrlApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

//Config Serilog
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs/Logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<EndPointValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EndPointIdValidator>();

builder.Services.AddScoped<IEndPointLogic, EndPointLogic>(); 
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEndPointDA, EndPointDA>();
builder.Services.AddScoped<DbConnection>(sp =>
{
    var connection = new SqliteConnection("Data Source=SQLite.db");
    connection.Open(); // keep it open for scoped lifetime
    return connection;
});


var app = builder.Build();
app.UseCors("TinyUrlApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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
