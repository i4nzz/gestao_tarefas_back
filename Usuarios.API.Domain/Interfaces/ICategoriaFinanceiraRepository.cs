using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

public interface ICategoriaFinanceiraRepository
{
    Task<IEnumerable<CategoriaFinanceira>> ObterTodasAsync();
    Task<CategoriaFinanceira?> ObterPorIdAsync(int id);
    Task AdicionarAsync(CategoriaFinanceira categoria);
    Task AtualizarAsync(CategoriaFinanceira categoria);
    Task RemoverAsync(int id);
}
