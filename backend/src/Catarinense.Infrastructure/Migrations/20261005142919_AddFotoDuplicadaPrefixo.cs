using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catarinense.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoDuplicadaPrefixo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProblemaExtraDescricao",
                table: "limpeza_etapa_execucoes",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProblemaExtraFotoDuplicadaOriginalPrefixo",
                table: "limpeza_etapa_execucoes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProblemaExtraFotoUrl",
                table: "limpeza_etapa_execucoes",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PrefixoOrigem",
                table: "FotosHashes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FotoDuplicadaOriginalPrefixo",
                table: "etapas_itens_execucao",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProblemaExtraDescricao",
                table: "limpeza_etapa_execucoes");

            migrationBuilder.DropColumn(
                name: "ProblemaExtraFotoDuplicadaOriginalPrefixo",
                table: "limpeza_etapa_execucoes");

            migrationBuilder.DropColumn(
                name: "ProblemaExtraFotoUrl",
                table: "limpeza_etapa_execucoes");

            migrationBuilder.DropColumn(
                name: "PrefixoOrigem",
                table: "FotosHashes");

            migrationBuilder.DropColumn(
                name: "FotoDuplicadaOriginalPrefixo",
                table: "etapas_itens_execucao");
        }
    }
}
