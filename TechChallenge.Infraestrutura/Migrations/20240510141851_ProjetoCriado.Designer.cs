﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TechChallenge.Infraestrutura.Data;

#nullable disable

namespace TechChallenge.Infraestrutura.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240510141851_ProjetoCriado")]
    partial class ProjetoCriado
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("App")
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Solucionadores", b =>
                {
                    b.Property<int>("AtividadesId")
                        .HasColumnType("int");

                    b.Property<int>("SolucionadoresId")
                        .HasColumnType("int");

                    b.HasKey("AtividadesId", "SolucionadoresId");

                    b.HasIndex("SolucionadoresId");

                    b.ToTable("Solucionadores", "App");
                });

            modelBuilder.Entity("TechChallenge.Dominio.Entities.Atividade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DepartamentoSolucionador")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EstahAtiva")
                        .HasColumnType("bit");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("PrazoEstimado")
                        .HasColumnType("bigint");

                    b.Property<int>("Prioridade")
                        .HasColumnType("int");

                    b.Property<int>("TipoDeDistribuicao")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Atividades", "App");
                });

            modelBuilder.Entity("TechChallenge.Dominio.Entities.Demanda", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AtividadeId")
                        .HasColumnType("int");

                    b.Property<string>("DepartamentoSolicitante")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DepartamentoSolucionador")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Detalhes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IdDaDemandaReaberta")
                        .HasColumnType("int");

                    b.Property<DateTime>("MomentoDeAbertura")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("MomentoDeFechamento")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Prazo")
                        .HasColumnType("datetime2");

                    b.Property<int>("Situacao")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioSolicitanteId")
                        .HasColumnType("int");

                    b.Property<int?>("UsuarioSolucionadorId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AtividadeId");

                    b.HasIndex("UsuarioSolicitanteId");

                    b.HasIndex("UsuarioSolucionadorId");

                    b.ToTable("Demandas", "App");
                });

            modelBuilder.Entity("TechChallenge.Dominio.Entities.EventoRegistrado", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("DemandaId")
                        .HasColumnType("int");

                    b.Property<string>("Mensagem")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("MomentoFinal")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("MomentoInicial")
                        .HasColumnType("datetime2");

                    b.Property<int>("Situacao")
                        .HasColumnType("int");

                    b.Property<int?>("UsuarioSolucionadorId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DemandaId");

                    b.HasIndex("UsuarioSolucionadorId");

                    b.ToTable("EventosRegistrados", "App");
                });

            modelBuilder.Entity("TechChallenge.Dominio.Entities.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Departamento")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EhGestor")
                        .HasColumnType("bit");

                    b.Property<string>("Matricula")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Usuarios", "App");
                });

            modelBuilder.Entity("Solucionadores", b =>
                {
                    b.HasOne("TechChallenge.Dominio.Entities.Atividade", null)
                        .WithMany()
                        .HasForeignKey("AtividadesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TechChallenge.Dominio.Entities.Usuario", null)
                        .WithMany()
                        .HasForeignKey("SolucionadoresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TechChallenge.Dominio.Entities.Demanda", b =>
                {
                    b.HasOne("TechChallenge.Dominio.Entities.Atividade", "Atividade")
                        .WithMany("Demandas")
                        .HasForeignKey("AtividadeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TechChallenge.Dominio.Entities.Usuario", "UsuarioSolicitante")
                        .WithMany()
                        .HasForeignKey("UsuarioSolicitanteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TechChallenge.Dominio.Entities.Usuario", "UsuarioSolucionador")
                        .WithMany()
                        .HasForeignKey("UsuarioSolucionadorId");

                    b.Navigation("Atividade");

                    b.Navigation("UsuarioSolicitante");

                    b.Navigation("UsuarioSolucionador");
                });

            modelBuilder.Entity("TechChallenge.Dominio.Entities.EventoRegistrado", b =>
                {
                    b.HasOne("TechChallenge.Dominio.Entities.Demanda", "Demanda")
                        .WithMany("EventosRegistrados")
                        .HasForeignKey("DemandaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TechChallenge.Dominio.Entities.Usuario", "UsuarioSolucionador")
                        .WithMany()
                        .HasForeignKey("UsuarioSolucionadorId");

                    b.Navigation("Demanda");

                    b.Navigation("UsuarioSolucionador");
                });

            modelBuilder.Entity("TechChallenge.Dominio.Entities.Atividade", b =>
                {
                    b.Navigation("Demandas");
                });

            modelBuilder.Entity("TechChallenge.Dominio.Entities.Demanda", b =>
                {
                    b.Navigation("EventosRegistrados");
                });
#pragma warning restore 612, 618
        }
    }
}
