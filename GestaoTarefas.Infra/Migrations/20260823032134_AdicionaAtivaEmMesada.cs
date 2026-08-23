using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaAtivaEmMesada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativa",
                table: "mesada",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativa",
                table: "mesada");
        }
    }
}
