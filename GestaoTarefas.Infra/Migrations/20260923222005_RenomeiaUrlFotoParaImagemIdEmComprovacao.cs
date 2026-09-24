using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RenomeiaUrlFotoParaImagemIdEmComprovacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UrlFoto",
                table: "comprovacao_tarefa",
                newName: "ImagemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagemId",
                table: "comprovacao_tarefa",
                newName: "UrlFoto");
        }
    }
}
