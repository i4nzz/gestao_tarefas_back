using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

public interface IRegistroFinanceiroRepository
{
    Task<IEnumerable<RegistroFinanceiro>> ObterPorFilhoAsync(int filhoId);
    Task<decimal> ObterTotalGastoPorMesadaAsync(int mesadaId);
    Task AdicionarAsync(RegistroFinanceiro registro);
}
