namespace GestaoTarefas.Application.DTOs.Mesada;

public class CriarMesadaDto
{
    public int FilhoId { get; set; }
    public decimal Valor { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
}
