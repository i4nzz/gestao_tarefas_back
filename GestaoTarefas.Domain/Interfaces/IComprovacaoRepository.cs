using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

public interface IComprovacaoRepository
{
    Task<IEnumerable<ComprovacaoTarefa>> ObterPorTarefaAsync(int tarefaId);
    Task<ComprovacaoTarefa?> ObterPorIdAsync(int id);
    Task<ComprovacaoTarefa?> ObterUltimaPorTarefaAsync(int tarefaId);
    Task AdicionarAsync(ComprovacaoTarefa comprovacao);
    Task AtualizarAsync(ComprovacaoTarefa comprovacao);
}
