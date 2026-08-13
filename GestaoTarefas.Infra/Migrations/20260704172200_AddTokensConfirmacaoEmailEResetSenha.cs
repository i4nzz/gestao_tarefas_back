using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddTokensConfirmacaoEmailEResetSenha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmado",
                table: "usuario",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TokenConfirmacaoEmail",
                table: "usuario",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenConfirmacaoExpiracao",
                table: "usuario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenResetSenha",
                table: "usuario",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenResetSenhaExpiracao",
                table: "usuario",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmado",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "TokenConfirmacaoEmail",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "TokenConfirmacaoExpiracao",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "TokenResetSenha",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "TokenResetSenhaExpiracao",
                table: "usuario");
        }
    }
}
