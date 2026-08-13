using GestaoTarefas.Application.DTOs.Pontuacao;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Application.Mapping;

public static class PontuacaoMapping
{
    public static RetornoPontuacaoDto ToDto(this Pontuacao pontuacao)
    {
        return new RetornoPontuacaoDto
        {
            Id = pontuacao.Id,
            FilhoId = pontuacao.FilhoId,
            TarefaId = pontuacao.TarefaId ?? 0,
            TituloTarefa = pontuacao.Tarefa?.Titulo ?? string.Empty,
            Pontos = pontuacao.Pontos,
            DataRegistro = pontuacao.DataRegistro
        };
    }

    public static IEnumerable<RetornoPontuacaoDto> ToDtoList(this IEnumerable<Pontuacao> pontuacoes)
    {
        return pontuacoes.Select(p => p.ToDto());
    }
}