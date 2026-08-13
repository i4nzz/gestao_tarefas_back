using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comprovacao_tarefa_tarefa_TarefaId1",
                table: "comprovacao_tarefa");

            migrationBuilder.DropIndex(
                name: "IX_comprovacao_tarefa_TarefaId1",
                table: "comprovacao_tarefa");

            migrationBuilder.DropColumn(
                name: "BeneficiarioTarefaId",
                table: "tarefa");

            migrationBuilder.DropColumn(
                name: "Preco",
                table: "tarefa");

            migrationBuilder.DropColumn(
                name: "TarefaId1",
                table: "comprovacao_tarefa");

            migrationBuilder.AddColumn<DateTime>(
                name: "Prazo",
                table: "tarefa",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prazo",
                table: "tarefa");

            migrationBuilder.AddColumn<int>(
                name: "BeneficiarioTarefaId",
                table: "tarefa",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Preco",
                table: "tarefa",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TarefaId1",
                table: "comprovacao_tarefa",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comprovacao_tarefa_TarefaId1",
                table: "comprovacao_tarefa",
                column: "TarefaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_comprovacao_tarefa_tarefa_TarefaId1",
                table: "comprovacao_tarefa",
                column: "TarefaId1",
                principalTable: "tarefa",
                principalColumn: "TarefaId");
        }
    }
}
