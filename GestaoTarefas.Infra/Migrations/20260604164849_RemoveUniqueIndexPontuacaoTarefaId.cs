using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueIndexPontuacaoTarefaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pontuacao_TarefaId",
                table: "pontuacao");

            migrationBuilder.CreateIndex(
                name: "IX_pontuacao_TarefaId",
                table: "pontuacao",
                column: "TarefaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pontuacao_TarefaId",
                table: "pontuacao");

            migrationBuilder.CreateIndex(
                name: "IX_pontuacao_TarefaId",
                table: "pontuacao",
                column: "TarefaId",
                unique: true,
                filter: "[TarefaId] IS NOT NULL");
        }
    }
}
