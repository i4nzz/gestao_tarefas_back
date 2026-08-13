using GestaoTarefas.Application.DTOs.Recompensa;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Application.Mapping;

public static class ComprovacaoMapping
{
    public static RetornoComprovacaoDto ToDto(this ComprovacaoTarefa comprovacao)
    {
        if (comprovacao is null)
            throw new ArgumentNullException(nameof(comprovacao));

        return new RetornoComprovacaoDto
        {
            Id = comprovacao.Id,
            TarefaId = comprovacao.TarefaId,
            TituloTarefa = comprovacao.Tarefa?.Titulo ?? string.Empty,
            Status = comprovacao.Status,
            DataEnvio = comprovacao.DataEnvio,
            DataValidacao = comprovacao.DataValidacao
        };
    }

    public static IEnumerable<RetornoComprovacaoDto> ToDtoList(this IEnumerable<ComprovacaoTarefa> comprovacoes)
    {
        if (comprovacoes is null)
            return Enumerable.Empty<RetornoComprovacaoDto>();

        return comprovacoes.Select(c => c.ToDto());
    }
}
