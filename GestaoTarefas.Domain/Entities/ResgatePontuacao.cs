using GestaoTarefas.Domain.Helper;

namespace GestaoTarefas.Domain.Entities;

public class ResgatePontuacao
{
    public int Id { get; set; }
    public int FilhoId { get; set; }
    public int RecompensaId { get; set; }
    public int Pontos { get; set; }
    public DateTime DataResgate { get; set; }

    public Usuario Filho { get; set; } = null!;
    public Recompensa Recompensa { get; set; } = null!;

    public static ResgatePontuacao Criar(int filhoId, int recompensaId, int pontos)
    {
        return new ResgatePontuacao
        {
            FilhoId = filhoId,
            RecompensaId = recompensaId,
            Pontos = pontos,
            DataResgate = DateTime.UtcNow.ToBrazilTime()
        };
    }
}