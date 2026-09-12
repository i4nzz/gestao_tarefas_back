using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RestringeDeleteTarefaEmPontuacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pontuacao_tarefa_TarefaId",
                table: "pontuacao");

            migrationBuilder.AddForeignKey(
                name: "FK_pontuacao_tarefa_TarefaId",
                table: "pontuacao",
                column: "TarefaId",
                principalTable: "tarefa",
                principalColumn: "TarefaId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pontuacao_tarefa_TarefaId",
                table: "pontuacao");

            migrationBuilder.AddForeignKey(
                name: "FK_pontuacao_tarefa_TarefaId",
                table: "pontuacao",
                column: "TarefaId",
                principalTable: "tarefa",
                principalColumn: "TarefaId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
