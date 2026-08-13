namespace GestaoTarefas.Application.DTOs.RegistroFinanceiro;

public class CriarRegistroFinanceiroDto
{
    public int FilhoId { get; set; }
    public int CategoriaId { get; set; }
    public int MesadaId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
