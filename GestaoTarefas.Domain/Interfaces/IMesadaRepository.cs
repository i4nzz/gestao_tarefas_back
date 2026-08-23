using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

public interface IMesadaRepository
{
    Task<IEnumerable<Mesada>> ObterPorFilhoAsync(int filhoId);
    Task<Mesada?> ObterPorIdAsync(int id);
    Task AdicionarAsync(Mesada mesada);
    Task AtualizarAsync(Mesada mesada);
    Task RemoverAsync(int id);
}
