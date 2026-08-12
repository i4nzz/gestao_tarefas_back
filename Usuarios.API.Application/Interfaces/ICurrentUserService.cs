using GestaoTarefas.Domain.Enum;

namespace GestaoTarefas.Application.Interfaces;

public interface ICurrentUserService
{
    int UsuarioId { get; }
    PerfilUsuarioEnum Perfil { get; }
}
