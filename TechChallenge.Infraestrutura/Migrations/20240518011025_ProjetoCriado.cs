using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechChallenge.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class ProjetoCriado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "App");

            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Atividades",
                schema: "App",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstahAtiva = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DepartamentoSolucionador = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoDeDistribuicao = table.Column<int>(type: "int", nullable: false),
                    Prioridade = table.Column<int>(type: "int", nullable: false),
                    PrazoEstimado = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atividades", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "App",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Matricula = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Departamento = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EhGestor = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Demandas",
                schema: "App",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AtividadeId = table.Column<int>(type: "int", nullable: false),
                    IdDaDemandaReaberta = table.Column<int>(type: "int", nullable: true),
                    MomentoDeAbertura = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MomentoDeFechamento = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Prazo = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Situacao = table.Column<int>(type: "int", nullable: false),
                    DepartamentoSolicitante = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioSolicitanteId = table.Column<int>(type: "int", nullable: false),
                    DepartamentoSolucionador = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioSolucionadorId = table.Column<int>(type: "int", nullable: true),
                    Detalhes = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demandas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Demandas_Atividades_AtividadeId",
                        column: x => x.AtividadeId,
                        principalSchema: "App",
                        principalTable: "Atividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Demandas_Usuarios_UsuarioSolicitanteId",
                        column: x => x.UsuarioSolicitanteId,
                        principalSchema: "App",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Demandas_Usuarios_UsuarioSolucionadorId",
                        column: x => x.UsuarioSolucionadorId,
                        principalSchema: "App",
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Solucionadores",
                schema: "App",
                columns: table => new
                {
                    AtividadesId = table.Column<int>(type: "int", nullable: false),
                    SolucionadoresId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solucionadores", x => new { x.AtividadesId, x.SolucionadoresId });
                    table.ForeignKey(
                        name: "FK_Solucionadores_Atividades_AtividadesId",
                        column: x => x.AtividadesId,
                        principalSchema: "App",
                        principalTable: "Atividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solucionadores_Usuarios_SolucionadoresId",
                        column: x => x.SolucionadoresId,
                        principalSchema: "App",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventosRegistrados",
                schema: "App",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DemandaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioSolucionadorId = table.Column<int>(type: "int", nullable: true),
                    Situacao = table.Column<int>(type: "int", nullable: false),
                    MomentoInicial = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MomentoFinal = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Mensagem = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosRegistrados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventosRegistrados_Demandas_DemandaId",
                        column: x => x.DemandaId,
                        principalSchema: "App",
                        principalTable: "Demandas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventosRegistrados_Usuarios_UsuarioSolucionadorId",
                        column: x => x.UsuarioSolucionadorId,
                        principalSchema: "App",
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Demandas_AtividadeId",
                schema: "App",
                table: "Demandas",
                column: "AtividadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Demandas_UsuarioSolicitanteId",
                schema: "App",
                table: "Demandas",
                column: "UsuarioSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Demandas_UsuarioSolucionadorId",
                schema: "App",
                table: "Demandas",
                column: "UsuarioSolucionadorId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosRegistrados_DemandaId",
                schema: "App",
                table: "EventosRegistrados",
                column: "DemandaId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosRegistrados_UsuarioSolucionadorId",
                schema: "App",
                table: "EventosRegistrados",
                column: "UsuarioSolucionadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Solucionadores_SolucionadoresId",
                schema: "App",
                table: "Solucionadores",
                column: "SolucionadoresId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventosRegistrados",
                schema: "App");

            migrationBuilder.DropTable(
                name: "Solucionadores",
                schema: "App");

            migrationBuilder.DropTable(
                name: "Demandas",
                schema: "App");

            migrationBuilder.DropTable(
                name: "Atividades",
                schema: "App");

            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "App");
        }
    }
}
