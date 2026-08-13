using GestaoTarefas.Domain.Enum;

namespace GestaoTarefas.Domain.Entities;

public class ComprovacaoTarefa
{
    public int Id { get; private set; }
    public int TarefaId { get; private set; }
    public Tarefa Tarefa { get; private set; }
    public string UrlFoto { get; private set; }
    public DateTime DataEnvio { get; private set; }
    public StatusValidacaoTarefaEnum Status { get; private set; }
    public DateTime? DataValidacao { get; private set; }

    protected ComprovacaoTarefa() { }

    public ComprovacaoTarefa(int tarefaId, string urlFoto)
    {
        TarefaId = tarefaId;
        UrlFoto = urlFoto;
        DataEnvio = DateTime.UtcNow;
        Status = StatusValidacaoTarefaEnum.Pendente;
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
