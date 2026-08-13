namespace GestaoTarefas.Domain.Entities;

public class Mesada
{
    public int MesadaId { get; set; }
    public int FilhoId { get; set; }
    public int? TarefaId { get; set; }
    public decimal Valor { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }

    // Relacionamentos
    public Usuario Filho { get; set; } = null!;
}