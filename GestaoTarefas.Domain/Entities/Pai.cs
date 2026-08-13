using GestaoTarefas.Domain.Enum;

namespace GestaoTarefas.Domain.Entities;

public class Pai : Usuario
{
    protected Pai() { }

    public Pai(string nome, string email, string senhaHash)
        : base(nome, email, senhaHash, PerfilUsuarioEnum.Pai)
    {
    }

    public ICollection<PaisFilhos> PaisFilhos { get; private set; } = new List<PaisFilhos>();
}