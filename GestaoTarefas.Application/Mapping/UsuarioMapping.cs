using GestaoTarefas.Application.DTOs.Usuario;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Application.Mapping;

public static class UsuarioMapping
{
    public static RetornoUsuarioDto ToDto(this Usuario usuario)
    {
        return new RetornoUsuarioDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Ativo = usuario.Ativo,
            Perfil = usuario.Perfil
        };
    }

    public static IEnumerable<RetornoUsuarioDto> ToDtoList(this IEnumerable<Usuario> usuarios)
    {
        return usuarios.Select(u => u.ToDto());
    }
}