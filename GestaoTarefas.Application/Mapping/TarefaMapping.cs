using GestaoTarefas.Application.DTOs.Tarefa;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Application.Mapping;

public static class TarefaMapping
{
    public static RetornoTarefaDto ToDto(this Tarefa tarefa)
    {
        return new RetornoTarefaDto
        {
            TarefaId = tarefa.TarefaId,
            FilhoId = tarefa.FilhoId,
            NomeFilho = tarefa.Filho?.Nome ?? string.Empty,
            Titulo = tarefa.Titulo,
            Descricao = tarefa.Descricao,
            Pontos = tarefa.Pontos,
            Prazo = tarefa.Prazo,
            DataCriacao = tarefa.DataCriacao
        };
    }

    public static IEnumerable<RetornoTarefaDto> ToDtoList(this IEnumerable<Tarefa> tarefas)
    {
        return tarefas.Select(t => t.ToDto());
    }
}