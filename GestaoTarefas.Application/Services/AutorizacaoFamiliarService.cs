using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;

namespace GestaoTarefas.Application.Services;

public class AutorizacaoFamiliarService : IAutorizacaoFamiliarService
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    public AutorizacaoFamiliarService(ICurrentUserService currentUser, IUsuarioRepository usuarioRepository)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
    }
    public async Task<bool> PodeAcessarFilhoAsync(int filhoId)
    {
        if (_currentUser.Perfil == PerfilUsuarioEnum.Filho)
        {
            return _currentUser.UsuarioId == filhoId;
        }

        return await _usuarioRepository.ExisteVinculoAsync(_currentUser.UsuarioId, filhoId);
    }
}
