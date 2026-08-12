using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Recompensa;

namespace GestaoTarefas.Application.Interfaces;

public interface IComprovacaoService
{
    Task<RespostaMetodos<IEnumerable<RetornoComprovacaoDto>>> ObterPorTarefaAsync(int tarefaId);
    Task<RespostaMetodos<RetornoComprovacaoDto?>> ObterPorIdAsync(int id);
    Task<RespostaMetodos<RetornoComprovacaoDto>> EnviarAsync(CriarComprovacaoDto dto);
    Task<RespostaMetodos<RetornoComprovacaoDto>> ValidarAsync(int id, bool aprovar);
    Task<RespostaMetodos<(byte[] Conteudo, string ContentType)?>> ObterFotoAsync(int comprovacaoId);
}
