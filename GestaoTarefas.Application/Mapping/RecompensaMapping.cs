using GestaoTarefas.Application.DTOs.Recompensa;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Helper;

namespace GestaoTarefas.Application.Mapping;

public static class RecompensaMapping
{
    public static RetornoRecompensaDto ToDto(this Recompensa recompensa)
    {
        return new RetornoRecompensaDto
        {
            Id = recompensa.Id,
            FilhoId = recompensa.FilhoId,
            NomeFilho = recompensa.Filho?.Nome ?? string.Empty,
            Descricao = recompensa.Descricao,
            PontosNecessarios = recompensa.PontosNecessarios,
            Ativa = recompensa.Ativa
        };
    }

    public static IEnumerable<RetornoRecompensaDto> ToDtoList(this IEnumerable<Recompensa> recompensas)
    {
        return recompensas.Select(r => r.ToDto());
    }

    public static RetornoRecompensaResgatadaDto ToDto(this RecompensaResgatada resgatada)
    {
        return new RetornoRecompensaResgatadaDto
        {
            Id = resgatada.Id,
            FilhoId = resgatada.FilhoId,
            NomeFilho = resgatada.Filho?.Nome ?? string.Empty,
            RecompensaId = resgatada.RecompensaId,
            DescricaoRecompensa = resgatada.Recompensa?.Descricao,
            PontosUtilizados = resgatada.Recompensa?.PontosNecessarios ?? 0,
            DataResgate = resgatada.DataResgate.ToBrazilTime()
        };
    }

    public static IEnumerable<RetornoRecompensaResgatadaDto> ToDtoListResgatadas(this IEnumerable<RecompensaResgatada> resgatadas)
    {
        return resgatadas.Select(r => r.ToDto());
    }
}
