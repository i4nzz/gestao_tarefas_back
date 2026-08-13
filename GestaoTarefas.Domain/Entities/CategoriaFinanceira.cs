namespace GestaoTarefas.Domain.Entities;

public class CategoriaFinanceira
{
    public int CategoriaFinanceiraId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public ICollection<RegistroFinanceiro> Registros { get; set; } = new List<RegistroFinanceiro>();
}
// dotnet ef migrations add newCreate --project GestaoTarefas.Infra --startup-project GestaoTarefas
// dotnet ef database update --project GestaoTarefas.Infra --startup-project GestaoTarefas
