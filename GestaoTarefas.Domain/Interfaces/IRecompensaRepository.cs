using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

public interface IRecompensaRepository
{
    Task<IEnumerable<Recompensa>> ObterPorFilhoAsync(int filhoId);
    Task<Recompensa?> ObterPorIdAsync(int id);
    Task AdicionarAsync(Recompensa recompensa);
    Task AtualizarAsync(Recompensa recompensa);
    Task RemoverAsync(int id);
    Task ResgatarAsync(RecompensaResgatada recompensaResgatada);
    Task<IEnumerable<RecompensaResgatada>> ObterResgatadasPorFilhoAsync(int filhoId);
}
