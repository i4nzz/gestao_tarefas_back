namespace GestaoTarefas.Application.DTOs.Tarefa;

public class CriarTarefaDto
{
    public int FilhoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Pontos { get; set; }
    public DateTime Prazo { get; set; }
}
