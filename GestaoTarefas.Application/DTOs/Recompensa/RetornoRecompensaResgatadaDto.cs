namespace GestaoTarefas.Application.DTOs.Recompensa;

public class RetornoRecompensaResgatadaDto
{
    public int Id { get; set; }
    public int FilhoId { get; set; }
    public string NomeFilho { get; set; } = string.Empty;
    public int RecompensaId { get; set; }
    public string? DescricaoRecompensa { get; set; }
    public int PontosUtilizados { get; set; }
    public DateTime DataResgate { get; set; }
}
