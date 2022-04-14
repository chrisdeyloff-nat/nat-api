using nat_api.data.context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using nat_api.config;
using Serilog;
using System.Reflection;
using AutoMapper;
using AutoMapper.Collection;
using AutoMapper.EquivalencyExpression;
using FluentValidation;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using nat_api.api.middleware;
using nat_api.api.pipeline_behaviors;

const string AllowReactFrontEnd = "AllowReactFrontEnd";

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

SettingsLoader.LoadSettings(config);

try
{
    Log.Information("Application Starting");
    var domainAssembly = Assembly.GetExecutingAssembly(); 

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<DataContext>(
                    options => options.UseNpgsql(Settings.DefaultConnection,
                                x => x.MigrationsAssembly("nat-api.data"))
                                .EnableSensitiveDataLogging(true));

    builder.Services.AddMediatR(domainAssembly);
    builder.Services.AddAutoMapper((serviceProvider, automapper) =>
    {
        automapper.AddCollectionMappers();
        automapper.UseEntityFrameworkCoreModel<DataContext>(serviceProvider);
    }, domainAssembly);
    builder.Services.AddFluentValidation(new[] { domainAssembly });
    builder.Services.AddValidatorsFromAssembly(domainAssembly);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: AllowReactFrontEnd,
                    builder => builder
                                .WithOrigins(Settings.UIOriginURL)
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials());
    });

    builder.Host.UseSerilog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors(AllowReactFrontEnd);

    app.UseAuthorization();

    app.UseMiddleware<ErrorHandlerMiddleware>();

    app.MapControllers();

    // create database and tables
    using (var scope = app.Services.CreateScope())
    using (var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>())
    {
        dataContext.Database.Migrate();
    }
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}