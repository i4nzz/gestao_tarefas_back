using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Application.Interfaces;

public interface ITokenService
{
    (string Token, DateTime Expiracao) GerarAccessToken(Usuario usuario);
    string GerarRefreshToken();
    TimeSpan ObterValidadeRefreshToken();
}
