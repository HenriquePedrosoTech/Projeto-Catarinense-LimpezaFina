using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catarinense.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOpcionaisEConcluida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "etapas_padrao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkVideo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Obrigatoria = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etapas_padrao", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FotosHashes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    Hash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotosHashes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "limpezas_finas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    OnibusId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    NumeroOS = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OperadorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IniciadaEm = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FinalizadaEm = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AvaliadaEm = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AvaliadorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_bin"),
                    ObservacaoAvaliacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NotificacaoEnviada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CortinasRetiradas = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_limpezas_finas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "onibus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    Prefixo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Placa = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_onibus", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    Matricula = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SenhaHash = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Perfil = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecisaTrocarSenha = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TentativasFalhasLogin = table.Column<int>(type: "int", nullable: false),
                    BloqueadoAte = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "etapas_itens_padrao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    EtapaPadraoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    Texto = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    EnquadramentoFoto = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Obrigatorio = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etapas_itens_padrao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_etapas_itens_padrao_etapas_padrao_EtapaPadraoId",
                        column: x => x.EtapaPadraoId,
                        principalTable: "etapas_padrao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "limpeza_etapa_execucoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    LimpezaFinaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    EtapaPadraoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    ConcluidaEm = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_limpeza_etapa_execucoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_limpeza_etapa_execucoes_etapas_padrao_EtapaPadraoId",
                        column: x => x.EtapaPadraoId,
                        principalTable: "etapas_padrao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_limpeza_etapa_execucoes_limpezas_finas_LimpezaFinaId",
                        column: x => x.LimpezaFinaId,
                        principalTable: "limpezas_finas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "etapas_itens_execucao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    LimpezaEtapaExecucaoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    EtapaItemPadraoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    Obrigatorio = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Funcionalidade = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RelatoProblema = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FotoUrl = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Concluida = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etapas_itens_execucao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_etapas_itens_execucao_etapas_itens_padrao_EtapaItemPadraoId",
                        column: x => x.EtapaItemPadraoId,
                        principalTable: "etapas_itens_padrao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_etapas_itens_execucao_limpeza_etapa_execucoes_LimpezaEtapaEx~",
                        column: x => x.LimpezaEtapaExecucaoId,
                        principalTable: "limpeza_etapa_execucoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "fotos_etapa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    LimpezaEtapaExecucaoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_bin"),
                    UrlArquivo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fotos_etapa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fotos_etapa_limpeza_etapa_execucoes_LimpezaEtapaExecucaoId",
                        column: x => x.LimpezaEtapaExecucaoId,
                        principalTable: "limpeza_etapa_execucoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_etapas_itens_execucao_EtapaItemPadraoId",
                table: "etapas_itens_execucao",
                column: "EtapaItemPadraoId");

            migrationBuilder.CreateIndex(
                name: "IX_etapas_itens_execucao_LimpezaEtapaExecucaoId",
                table: "etapas_itens_execucao",
                column: "LimpezaEtapaExecucaoId");

            migrationBuilder.CreateIndex(
                name: "IX_etapas_itens_padrao_EtapaPadraoId",
                table: "etapas_itens_padrao",
                column: "EtapaPadraoId");

            migrationBuilder.CreateIndex(
                name: "IX_fotos_etapa_LimpezaEtapaExecucaoId",
                table: "fotos_etapa",
                column: "LimpezaEtapaExecucaoId");

            migrationBuilder.CreateIndex(
                name: "IX_limpeza_etapa_execucoes_EtapaPadraoId",
                table: "limpeza_etapa_execucoes",
                column: "EtapaPadraoId");

            migrationBuilder.CreateIndex(
                name: "IX_limpeza_etapa_execucoes_LimpezaFinaId",
                table: "limpeza_etapa_execucoes",
                column: "LimpezaFinaId");

            migrationBuilder.CreateIndex(
                name: "IX_limpezas_finas_NumeroOS",
                table: "limpezas_finas",
                column: "NumeroOS");

            migrationBuilder.CreateIndex(
                name: "IX_limpezas_finas_OnibusId",
                table: "limpezas_finas",
                column: "OnibusId");

            migrationBuilder.CreateIndex(
                name: "IX_limpezas_finas_OperadorId",
                table: "limpezas_finas",
                column: "OperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_limpezas_finas_Status",
                table: "limpezas_finas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_onibus_Prefixo",
                table: "onibus",
                column: "Prefixo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Matricula",
                table: "usuarios",
                column: "Matricula",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "etapas_itens_execucao");

            migrationBuilder.DropTable(
                name: "fotos_etapa");

            migrationBuilder.DropTable(
                name: "FotosHashes");

            migrationBuilder.DropTable(
                name: "onibus");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "etapas_itens_padrao");

            migrationBuilder.DropTable(
                name: "limpeza_etapa_execucoes");

            migrationBuilder.DropTable(
                name: "etapas_padrao");

            migrationBuilder.DropTable(
                name: "limpezas_finas");
        }
    }
}
