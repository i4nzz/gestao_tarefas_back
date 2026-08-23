using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Mesada;

namespace GestaoTarefas.Application.Interfaces;

public interface IMesadaService
{
    Task<RespostaMetodos<IEnumerable<RetornoMesadaDto>>> ObterPorFilhoAsync(int filhoId);
    Task<RespostaMetodos<RetornoMesadaDto>> CriarAsync(CriarMesadaDto dto);
    Task<RespostaMetodos<RetornoMesadaDto>> AtualizarAsync(int id, AtualizarMesadaDto dto);
    Task<RespostaMetodos<RetornoMesadaDto>> RemoverAsync(int id);
}
