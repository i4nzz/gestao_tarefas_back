using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoTarefas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AlterandoValidadaParaStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Cria a nova coluna
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "comprovacao_tarefa",
                type: "int",
                nullable: false,
                defaultValue: 1); // Pendente

            // 2. Converte os dados antigos
            migrationBuilder.Sql(@"
        UPDATE comprovacao_tarefa
        SET Status = CASE 
            WHEN Validada = 1 THEN 2 -- Aprovada
            ELSE 1 -- Pendente
        END
    ");

            // 3. Remove a coluna antiga
            migrationBuilder.DropColumn(
                name: "Validada",
                table: "comprovacao_tarefa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Volta coluna antiga
            migrationBuilder.AddColumn<bool>(
                name: "Validada",
                table: "comprovacao_tarefa",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // 2. Converte de volta
            migrationBuilder.Sql(@"
        UPDATE comprovacao_tarefa
        SET Validada = CASE 
            WHEN Status = 2 THEN 1
            ELSE 0
        END
    ");

            // 3. Remove Status
            migrationBuilder.DropColumn(
                name: "Status",
                table: "comprovacao_tarefa");
        }
    }
}
