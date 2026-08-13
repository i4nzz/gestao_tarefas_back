using GestaoTarefas.Domain.Enum;

namespace GestaoTarefas.Domain.Entities;

public class Pontuacao
{
    public int Id { get; set; }
    public int FilhoId { get; set; }
    public int? TarefaId { get; set; }
    public int Pontos { get; set; }
    public DateTime DataRegistro { get; set; } = DateTime.UtcNow;

    public Usuario Filho { get; set; } = null!;
    public Tarefa? Tarefa { get; set; } = null!;
    public TipoPontuacaoEnum Tipo { get; set; }

    public static Pontuacao CriarGanho(int filhoId, int tarefaId, int pontos)
    {
        return new Pontuacao
        {
            FilhoId = filhoId,
            TarefaId = tarefaId,
            Pontos = pontos,
            Tipo = TipoPontuacaoEnum.Ganho
        };
    }
}
