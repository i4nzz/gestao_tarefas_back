using GestaoTarefas.Domain.Enum;

namespace GestaoTarefas.Application.DTOs.Recompensa;

public class RetornoComprovacaoDto
{
    public int Id { get; set; }
    public int TarefaId { get; set; }
    public string TituloTarefa { get; set; } = string.Empty;
    public StatusValidacaoTarefaEnum Status { get; set; }
    public DateTime DataEnvio { get; set; }
    public DateTime? DataValidacao { get; set; }
}
