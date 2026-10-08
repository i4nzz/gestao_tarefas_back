using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Questionario;

namespace GestaoTarefas.Application.Interfaces;

public interface IQuestionarioService
{
    Task<RespostaMetodos<IEnumerable<RetornoPerguntaQuestionarioDto>>> IniciarAsync(int? quantidade);
    Task<RespostaMetodos<RetornoRespostaQuestionarioDto>> ResponderAsync(ResponderPerguntaDto dto);
}
