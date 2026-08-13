namespace GestaoTarefas.Domain.Entities;

public class Tarefa
{
    public int TarefaId { get; set; }
    public int FilhoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Pontos { get; set; }
    public DateTime Prazo { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public Usuario Filho { get; set; } = null!;
    public ICollection<ComprovacaoTarefa> Comprovacoes { get; set; } = new List<ComprovacaoTarefa>();
}
