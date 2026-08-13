namespace GestaoTarefas.Domain.Entities;

public class Recompensa
{
    public int Id { get; set; }
    public int FilhoId { get; set; }
    public string? Descricao { get; set; }
    public int PontosNecessarios { get; set; }
    public bool Ativa { get; set; } = true;

    public Usuario Filho { get; set; } = null!;
    public ICollection<RecompensaResgatada> RecompensasResgatadas { get; set; } = new List<RecompensaResgatada>();
}
