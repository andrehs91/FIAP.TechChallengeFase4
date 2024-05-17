using TechChallenge.Dominio.Interfaces;
using TechChallenge.Dominio.Policies;
using TechChallenge.Infraestrutura.Data;
using TechChallenge.Infraestrutura.Repositories;
using TechChallenge.Worker;
using TechChallenge.Worker.Configurations;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IAppSettings, AppSettings>();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<IAtividadeRepository, AtividadeRepository>();
builder.Services.AddScoped<IDemandaRepository, DemandaRepository>();
builder.Services.AddScoped<ISolucionadorPolicy, SolucionadorPolicy>();

var host = builder.Build();
host.Run();
