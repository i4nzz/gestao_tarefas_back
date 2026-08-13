namespace GestaoTarefas.Domain.Entities;

public class RecompensaResgatada
{
    public int Id { get; private set; }

    public int FilhoId { get; private set; }
    public Filho Filho { get; private set; }

    public int RecompensaId { get; private set; }
    public Recompensa Recompensa { get; private set; }

    public DateTime DataResgate { get; private set; }

    protected RecompensaResgatada() { }

    public RecompensaResgatada(int filhoId, int recompensaId)
    {
        FilhoId = filhoId;
        RecompensaId = recompensaId;
        DataResgate = DateTime.UtcNow;
    }
}
