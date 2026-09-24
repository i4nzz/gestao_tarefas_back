using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

/// <summary>
/// Repositório responsável por persistir as imagens de comprovação de tarefa (bytes + data/hora de
/// salvamento) em um armazenamento próprio para binários, dissociado do banco relacional principal.
/// </summary>
public interface IImagemComprovacaoRepository
{
    /// <summary>
    /// Salva uma nova imagem e retorna o identificador gerado para ela.
    /// </summary>
    Task<string> SalvarAsync(byte[] conteudo, string contentType, string nomeArquivoOriginal);

    /// <summary>
    /// Obtém uma imagem previamente salva pelo seu identificador.
    /// </summary>
    Task<ImagemComprovacao?> ObterPorIdAsync(string id);

    /// <summary>
    /// Substitui o conteúdo de uma imagem já existente, atualizando também a data/hora de salvamento.
    /// </summary>
    Task AtualizarAsync(string id, byte[] conteudo, string contentType, string nomeArquivoOriginal);

    /// <summary>
    /// Remove uma imagem do armazenamento.
    /// </summary>
    Task RemoverAsync(string id);
}
