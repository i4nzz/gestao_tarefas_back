using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddResgatePontuacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "resgate_pontuacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilhoId = table.Column<int>(type: "int", nullable: false),
                    RecompensaId = table.Column<int>(type: "int", nullable: false),
                    Pontos = table.Column<int>(type: "int", nullable: false),
                    DataResgate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resgate_pontuacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resgate_pontuacao_recompensa_RecompensaId",
                        column: x => x.RecompensaId,
                        principalTable: "recompensa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_resgate_pontuacao_usuario_FilhoId",
                        column: x => x.FilhoId,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_resgate_pontuacao_FilhoId",
                table: "resgate_pontuacao",
                column: "FilhoId");

            migrationBuilder.CreateIndex(
                name: "IX_resgate_pontuacao_RecompensaId",
                table: "resgate_pontuacao",
                column: "RecompensaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resgate_pontuacao");
        }
    }
}
