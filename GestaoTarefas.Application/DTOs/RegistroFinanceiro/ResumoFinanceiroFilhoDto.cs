namespace GestaoTarefas.Application.DTOs.RegistroFinanceiro;

public class ResumoFinanceiroFilhoDto
{
    public int FilhoId { get; set; }
    public string NomeFilho { get; set; } = string.Empty;
    public decimal TotalMesadas { get; set; }
    public decimal TotalGasto { get; set; }
    public decimal SaldoDisponivel { get; set; }
    public List<GastoPorCategoriaDto> GastosPorCategoria { get; set; } = new();
}
