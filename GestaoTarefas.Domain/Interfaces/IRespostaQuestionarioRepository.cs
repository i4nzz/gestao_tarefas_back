using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

/// <summary>
/// Histórico de respostas do questionário (coleção "respostasQuestionario" do MongoDB).
/// </summary>
public interface IRespostaQuestionarioRepository
{
    Task RegistrarAsync(RespostaQuestionario resposta);

    /// <summary>
    /// Respostas do filho, da mais recente para a mais antiga. Sem <paramref name="desde"/>, traz todas.
    /// </summary>
    Task<IEnumerable<RespostaQuestionario>> ObterPorFilhoAsync(int filhoId, DateTime? desde);
}
