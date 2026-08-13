using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class TPTHierarquia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuario_usuario_PaiId",
                table: "usuario");

            migrationBuilder.DropIndex(
                name: "IX_usuario_PaiId",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "PaiId",
                table: "usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaiId",
                table: "usuario",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_PaiId",
                table: "usuario",
                column: "PaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_usuario_usuario_PaiId",
                table: "usuario",
                column: "PaiId",
                principalTable: "usuario",
                principalColumn: "Id");
        }
    }
}
