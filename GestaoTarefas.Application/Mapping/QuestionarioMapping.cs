using GestaoTarefas.Application.DTOs.Questionario;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Application.Mapping;

public static class QuestionarioMapping
{
    public static RetornoPerguntaQuestionarioDto ToDto(this PerguntaQuestionario pergunta)
    {
        return new RetornoPerguntaQuestionarioDto
        {
            Id = pergunta.Id,
            Categoria = pergunta.Categoria,
            Dificuldade = pergunta.Dificuldade,
            Pergunta = pergunta.Pergunta,
            Opcoes = pergunta.Opcoes,
            Pontos = pergunta.PontosPorAcerto
        };
    }
}
