namespace GestaoTarefas.Domain.Entities;

public class RegistroFinanceiro
{
    public int RegistroId { get; set; }
    public int FilhoId { get; set; }
    public int CategoriaId { get; set; }
    public int MesadaId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataRegistro { get; set; } = DateTime.UtcNow;
    public Usuario Filho { get; set; } = null!;
    public CategoriaFinanceira Categoria { get; set; } = null!;
    public Mesada Mesada { get; set; } = null!;
}
