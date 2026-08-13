using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoPontuacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pontuacao_TarefaId",
                table: "pontuacao");

            migrationBuilder.AlterColumn<int>(
                name: "TarefaId",
                table: "pontuacao",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "pontuacao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_pontuacao_TarefaId",
                table: "pontuacao",
                column: "TarefaId",
                unique: true,
                filter: "[TarefaId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pontuacao_TarefaId",
                table: "pontuacao");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "pontuacao");

            migrationBuilder.AlterColumn<int>(
                name: "TarefaId",
                table: "pontuacao",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_pontuacao_TarefaId",
                table: "pontuacao",
                column: "TarefaId",
                unique: true);
        }
    }
}
