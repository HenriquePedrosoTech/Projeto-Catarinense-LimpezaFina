using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catarinense.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescricaoEtapaItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "etapas_itens_padrao",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "etapas_itens_padrao");
        }
    }
}
