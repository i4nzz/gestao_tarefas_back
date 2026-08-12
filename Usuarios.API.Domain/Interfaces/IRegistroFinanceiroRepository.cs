using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

public interface IRegistroFinanceiroRepository
{
    Task<IEnumerable<RegistroFinanceiro>> ObterPorFilhoAsync(int filhoId);
    Task AdicionarAsync(RegistroFinanceiro registro);
}
