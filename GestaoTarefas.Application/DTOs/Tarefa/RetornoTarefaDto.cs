namespace GestaoTarefas.Application.DTOs.Tarefa;

public class RetornoTarefaDto
{
    public int TarefaId { get; set; }
    public int FilhoId { get; set; }
    public string NomeFilho { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Pontos { get; set; }
    public DateTime Prazo { get; set; }
    public DateTime DataCriacao { get; set; }
}
