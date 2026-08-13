using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<IEnumerable<Usuario>> ObterTodosAsync();
    Task AdicionarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
    Task RemoverAsync(int id);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task AdicionarFilhoAsync(Filho filho, PaisFilhos vinculo);
    Task<Usuario?> ObterPorTokenConfirmacaoEmailAsync(string token);
    Task<Usuario?> ObterPorTokenResetSenhaAsync(string token);
    Task<bool> ExisteVinculoAsync(int paiId, int filhoId);
    Task<bool> PossuiVinculoFamiliarAsync(int usuarioId);
}

