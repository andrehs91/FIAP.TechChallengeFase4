using Microsoft.EntityFrameworkCore;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.Dominio.Policies;
using TechChallenge.Infraestrutura.Data;
using TechChallenge.Infraestrutura.Repositories;
using TechChallenge.Worker;
using TechChallenge.Worker.Configurations;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

AppSettings appSettings = new(builder.Configuration);
string server = appSettings.GetValue("MariaDB:Server");
string database = appSettings.GetValue("MariaDB:Database");
string user = appSettings.GetValue("MariaDB:User");
string password = appSettings.GetValue("MariaDB:Password");
string connectionString = $"server={server}; database={database}; user={user}; password={password}";
builder.Services.AddDbContext<ApplicationDbContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .LogTo(Console.WriteLine, LogLevel.Debug)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

builder.Services.AddSingleton<IAppSettings, AppSettings>();
builder.Services.AddScoped<IAtividadeRepository, AtividadeRepository>();
builder.Services.AddScoped<IDemandaRepository, DemandaRepository>();
builder.Services.AddScoped<ISolucionadorPolicy, SolucionadorPolicy>();

var host = builder.Build();
host.Run();
