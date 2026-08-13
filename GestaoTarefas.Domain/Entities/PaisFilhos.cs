namespace GestaoTarefas.Domain.Entities;

// entidade de relacionamento muitos-para-muitos entre Pai e Filho
public class PaisFilhos
{
    public int PaiId { get; private set; }
    public Pai Pai { get; private set; }

    public int FilhoId { get; private set; }
    public Filho Filho { get; private set; }

    public DateTime DataVinculo { get; private set; }

    protected PaisFilhos() { }

    public PaisFilhos(int paiId, int filhoId)
    {
        PaiId = paiId;
        FilhoId = filhoId;
        DataVinculo = DateTime.UtcNow;
    }
}
