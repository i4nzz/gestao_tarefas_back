using System.Security.Claims;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Domain.Enum;
using Microsoft.AspNetCore.Http;

namespace GestaoTarefas.Application.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UsuarioId
    {
        get
        {
            var valor = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(valor, out var id) ? id : throw new UnauthorizedAccessException("Usuário não autenticado.");
        }
    }

    public PerfilUsuarioEnum Perfil
    {
        get
        {
            var valor = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<PerfilUsuarioEnum>(valor, out var perfil) ? perfil : throw new UnauthorizedAccessException("Usuário não autenticado.");
        }
    }
}
