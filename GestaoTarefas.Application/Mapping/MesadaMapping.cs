using GestaoTarefas.Application.DTOs.Mesada;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Application.Mapping;

public static class MesadaMapping
{
    public static RetornoMesadaDto ToDto(this Mesada mesada, decimal valorGasto = 0)
    {
        return new RetornoMesadaDto
        {
            MesadaId = mesada.MesadaId,
            FilhoId = mesada.FilhoId,
            NomeFilho = mesada.Filho?.Nome ?? string.Empty,
            Valor = mesada.Valor,
            Mes = mesada.Mes,
            Ano = mesada.Ano,
            ValorGasto = valorGasto,
            SaldoDisponivel = mesada.Valor - valorGasto,
            Ativa = mesada.Ativa
        };
    }
}
