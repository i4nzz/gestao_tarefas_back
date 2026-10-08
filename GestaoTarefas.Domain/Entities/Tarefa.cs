using GestaoTarefas.Domain.Enum;

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
    public bool Arquivada { get; set; }
    public Usuario Filho { get; set; } = null!;
    public ICollection<ComprovacaoTarefa> Comprovacoes { get; set; } = new List<ComprovacaoTarefa>();

    /// <summary>
    /// Status calculado a partir do prazo e das comprovações; não é gravado no banco.
    /// </summary>
    public StatusTarefaEnum CalcularStatus(DateTime agoraUtc)
    {
        if (Comprovacoes.Any(c => c.Status == StatusValidacaoTarefaEnum.Aprovada))
        {
            return StatusTarefaEnum.Concluida;
        }

        if (Comprovacoes.Any(c => c.Status == StatusValidacaoTarefaEnum.Pendente))
        {
            return StatusTarefaEnum.AguardandoValidacao;
        }

        if (Prazo < agoraUtc)
        {
            return StatusTarefaEnum.Expirada;
        }

        return StatusTarefaEnum.Pendente;
    }
}
