using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> ObterPorTokenAsync(string token);
    Task AdicionarAsync(RefreshToken refreshToken);
    Task AtualizarAsync(RefreshToken refreshToken);
}
