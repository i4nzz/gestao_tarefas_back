namespace GestaoTarefas.Application.DTOs.RegistroFinanceiro;

public class GastoPorCategoriaDto
{
    public int CategoriaId { get; set; }
    public string NomeCategoria { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public double Percentual { get; set; }
}
