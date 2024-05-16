using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TechChallenge.Dominio.Entities;

namespace TechChallenge.Infraestrutura.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : DbContext(options)
{
    private readonly IConfiguration _configuration = configuration;

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Atividade> Atividades { get; set; } = null!;
    public DbSet<Demanda> Demandas { get; set; } = null!;
    public DbSet<EventoRegistrado> EventosRegistrados { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SQLServer"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("App");

        modelBuilder.Entity<Atividade>()
            .HasMany(atividade => atividade.Solucionadores)
            .WithMany(usuario => usuario.Atividades)
            .UsingEntity("Solucionadores");

        modelBuilder.Entity<Atividade>()
            .HasMany(atividade => atividade.Demandas)
            .WithOne(demanda => demanda.Atividade)
            .HasForeignKey(demanda => demanda.AtividadeId)
            .IsRequired();

        modelBuilder.Entity<Demanda>()
            .HasMany(demanda => demanda.EventosRegistrados)
            .WithOne(eventoRegistrado => eventoRegistrado.Demanda)
            .HasForeignKey(eventoRegistrado => eventoRegistrado.DemandaId)
            .IsRequired();
    }
}
