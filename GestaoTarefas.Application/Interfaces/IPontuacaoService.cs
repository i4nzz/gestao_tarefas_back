using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Pontuacao;

namespace GestaoTarefas.Application.Interfaces;

public interface IPontuacaoService
{
    Task<RespostaMetodos<IEnumerable<RetornoPontuacaoDto>>> ObterPorFilhoAsync(int filhoId);
    Task<RespostaMetodos<int>> ObterTotalPontosAsync(int filhoId);
    Task<RespostaMetodos<RetornoPontuacaoDto>> AdicionarAsync(CriarPontuacaoDto dto);
}
