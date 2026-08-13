using GestaoTarefas.Application.DTOs.RegistroFinanceiro;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Application.Mapping;

public static class RegistroFinanceiroMapping
{
    public static RetornoRegistroFinanceiroDto ToDto(this RegistroFinanceiro registro)
    {
        return new RetornoRegistroFinanceiroDto
        {
            RegistroId = registro.RegistroId,
            FilhoId = registro.FilhoId,
            NomeFilho = registro.Filho?.Nome ?? string.Empty,
            CategoriaId = registro.CategoriaId,
            NomeCategoria = registro.Categoria?.Nome ?? string.Empty,
            MesadaId = registro.MesadaId,
            Descricao = registro.Descricao,
            Valor = registro.Valor,
            DataRegistro = registro.DataRegistro
        };
    }

    public static IEnumerable<RetornoRegistroFinanceiroDto> ToDtoList(this IEnumerable<RegistroFinanceiro> registros)
    {
        return registros.Select(r => r.ToDto());
    }
}
