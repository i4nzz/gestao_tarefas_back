using GestaoTarefas.Application.DTOs.CategoriaFinanceira;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Application.Mapping;

public static class CategoriaFinanceiraMapping
{
    public static RetornoCategoriaFinanceiraDto ToDto(this CategoriaFinanceira categoria)
    {
        return new RetornoCategoriaFinanceiraDto
        {
            CategoriaFinanceiraId = categoria.CategoriaFinanceiraId,
            Nome = categoria.Nome
        };
    }

    public static IEnumerable<RetornoCategoriaFinanceiraDto> ToDtoList(this IEnumerable<CategoriaFinanceira> categorias)
    {
        return categorias.Select(c => c.ToDto());
    }
}
