using GestaoTarefas.Domain.Enum;

namespace GestaoTarefas.Domain.Entities;

public class Filho : Usuario
{
    public DateTime DataNascimento { get; private set; }
    public ICollection<PaisFilhos> PaisFilhos { get; private set; } = new List<PaisFilhos>();

    protected Filho() { }

    public Filho(string nome, string email, string senhaHash, DateTime dataNascimento)
        : base(nome, email, senhaHash, PerfilUsuarioEnum.Filho)
    {
        DataNascimento = dataNascimento;
    }
}