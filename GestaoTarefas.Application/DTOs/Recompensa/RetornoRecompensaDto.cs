namespace GestaoTarefas.Application.DTOs.Recompensa;

public class RetornoRecompensaDto
{
    public int Id { get; set; }
    public int FilhoId { get; set; }
    public string NomeFilho { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int PontosNecessarios { get; set; }
    public bool Ativa { get; set; }
}
