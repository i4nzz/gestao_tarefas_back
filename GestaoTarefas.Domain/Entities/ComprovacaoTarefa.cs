using GestaoTarefas.Domain.Enum;

namespace GestaoTarefas.Domain.Entities;

public class ComprovacaoTarefa
{
    public int Id { get; private set; }
    public int TarefaId { get; private set; }
    public Tarefa Tarefa { get; private set; }
    public string ImagemId { get; private set; }
    public DateTime DataEnvio { get; private set; }
    public StatusValidacaoTarefaEnum Status { get; private set; }
    public DateTime? DataValidacao { get; private set; }

    protected ComprovacaoTarefa() { }

    public ComprovacaoTarefa(int tarefaId, string imagemId)
    {
        TarefaId = tarefaId;
        ImagemId = imagemId;
        DataEnvio = DateTime.UtcNow;
        Status = StatusValidacaoTarefaEnum.Pendente;
    }

    /// <summary>
    /// Marca a foto desta comprovação como substituída por uma nova (o conteúdo da imagem já foi
    /// sobrescrito no armazenamento). Só é permitido enquanto a comprovação ainda não foi validada.
    /// </summary>
    public void SubstituirFoto()
    {
        if (Status != StatusValidacaoTarefaEnum.Pendente)
            throw new InvalidOperationException("Não é possível enviar uma nova foto para uma comprovação que já foi aprovada ou reprovada.");

        DataEnvio = DateTime.UtcNow;
    }

    public void Aprovar()
    {
        if (Status == StatusValidacaoTarefaEnum.Aprovada)
            throw new InvalidOperationException("Comprovação já foi validada");

        Status = StatusValidacaoTarefaEnum.Aprovada;
        DataValidacao = DateTime.UtcNow;
    }

    public void Reprovar()
    {
        if (Status == StatusValidacaoTarefaEnum.Aprovada)
            throw new InvalidOperationException("Comprovação já foi aprovada e não pode ser reprovada");

        Status = StatusValidacaoTarefaEnum.Reprovada;
        DataValidacao = DateTime.UtcNow;
    }
}
