using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

/// <summary>
/// Repositório das perguntas do questionário gamificado (coleção "bancoQuestionario" do MongoDB).
/// </summary>
public interface IBancoQuestionarioRepository
{
    /// <summary>
    /// Sorteia até <paramref name="quantidade"/> perguntas ativas, sem repetição.
    /// </summary>
    Task<IEnumerable<PerguntaQuestionario>> ObterAleatoriasAsync(int quantidade);

    /// <summary>
    /// Obtém uma pergunta pelo seu identificador.
    /// </summary>
    Task<PerguntaQuestionario?> ObterPorIdAsync(string id);
}
