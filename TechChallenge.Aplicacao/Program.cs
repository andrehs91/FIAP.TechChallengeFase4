using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Text;
using System.Text.Json.Serialization;
using TechChallenge.Aplicacao.Commands;
using TechChallenge.Aplicacao.Configurations;
using TechChallenge.Aplicacao.Services;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.Dominio.Policies;
using TechChallenge.Infraestrutura.Cache;
using TechChallenge.Infraestrutura.Data;
using TechChallenge.Infraestrutura.Repositories;
using TechChallenge.Infraestrutura.Settings;

var builder = WebApplication.CreateBuilder(args);

AppSettings appSettings = new(builder.Configuration);
string server = appSettings.GetValue("MariaDB:Server");
string database = appSettings.GetValue("MariaDB:Database");
string user = appSettings.GetValue("MariaDB:User");
string password = appSettings.GetValue("MariaDB:Password");
string connectionString = $"server={server}; database={database}; user={user}; password={password}";
builder.Services.AddDbContext<ApplicationDbContext>(
    dbContextOptions => dbContextOptions.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mysqlOptions => { mysqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore); }
    )
);

builder.Services.AddSingleton<IAppSettings, AppSettings>();
builder.Services.AddScoped<IRedisCache, RedisCache>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAtividadeRepository, AtividadeRepository>();
builder.Services.AddScoped<IDemandaRepository, DemandaRepository>();
builder.Services.AddScoped<ISolucionadorPolicy, SolucionadorPolicy>();
builder.Services.AddScoped<UsuarioCommand>();
builder.Services.AddScoped<AtividadeCommand>();
builder.Services.AddScoped<DemandaCommand>();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Secret"]!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    o.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context => Handlers.OnAuthenticationFailedHandler(context),
        OnForbidden = context => Handlers.OnForbiddenHandler(context)
    };
});
builder.Services.AddSwaggerGen(o => AppOptions.SwaggerGenOptions(o));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}

app.UseExceptionHandler(AppOptions.ExceptionHandlerOptions());
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
