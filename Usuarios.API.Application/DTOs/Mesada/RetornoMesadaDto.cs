namespace GestaoTarefas.Application.DTOs.Mesada;

public class RetornoMesadaDto
{
    public int MesadaId { get; set; }
    public int FilhoId { get; set; }
    public string NomeFilho { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
}
