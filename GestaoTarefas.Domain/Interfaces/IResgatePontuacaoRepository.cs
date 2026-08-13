using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

public interface IResgatePontuacaoRepository
{
    Task AdicionarAsync(ResgatePontuacao resgate);
    Task<IEnumerable<ResgatePontuacao>> ObterPorFilhoAsync(int filhoId);
    Task<int> ObterTotalResgatesAsync(int filhoId);
}
