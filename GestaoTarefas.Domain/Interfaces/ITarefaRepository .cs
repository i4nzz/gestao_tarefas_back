using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

public interface ITarefaRepository
{
    Task<IEnumerable<Tarefa>> ObterTodasAsync();
    Task<IEnumerable<Tarefa>> ObterPorFilhoAsync(int filhoId);
    Task<Tarefa?> ObterPorIdAsync(int tarefaId);
    Task AdicionarAsync(Tarefa tarefa);
    Task AtualizarAsync(Tarefa tarefa);
    Task RemoverAsync(int tarefaId);
}
