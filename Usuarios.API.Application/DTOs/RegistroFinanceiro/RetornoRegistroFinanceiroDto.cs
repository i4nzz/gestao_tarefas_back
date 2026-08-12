namespace GestaoTarefas.Application.DTOs.RegistroFinanceiro;

public class RetornoRegistroFinanceiroDto
{
    public int RegistroId { get; set; }
    public int FilhoId { get; set; }
    public string NomeFilho { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string NomeCategoria { get; set; } = string.Empty;
    public int MesadaId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataRegistro { get; set; }
}
