using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Tarefa;

namespace GestaoTarefas.Application.Interfaces;

public interface ITarefaService
{
    Task<RespostaMetodos<IEnumerable<RetornoTarefaDto>>> ObterTodasAsync();
    Task<RespostaMetodos<IEnumerable<RetornoTarefaDto>>> ObterPorFilhoAsync(int filhoId);
    Task<RespostaMetodos<RetornoTarefaDto?>> ObterPorIdAsync(int tarefaId);
    Task<RespostaMetodos<RetornoTarefaDto>> CriarAsync(CriarTarefaDto dto);
    Task<RespostaMetodos<RetornoTarefaDto?>> AtualizarAsync(int tarefaId, CriarTarefaDto dto);
    Task<RespostaMetodos<RetornoTarefaDto?>> RemoverAsync(int tarefaId);
}
