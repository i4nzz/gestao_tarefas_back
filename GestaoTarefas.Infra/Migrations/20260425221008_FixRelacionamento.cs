using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class FixRelacionamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataRegistro",
                table: "recompensa");

            migrationBuilder.DropColumn(
                name: "PodeNecessariosInt",
                table: "recompensa");

            migrationBuilder.DropColumn(
                name: "TarefaId",
                table: "recompensa");

            migrationBuilder.AddColumn<bool>(
                name: "Ativa",
                table: "recompensa",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativa",
                table: "recompensa");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataRegistro",
                table: "recompensa",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "PodeNecessariosInt",
                table: "recompensa",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TarefaId",
                table: "recompensa",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
