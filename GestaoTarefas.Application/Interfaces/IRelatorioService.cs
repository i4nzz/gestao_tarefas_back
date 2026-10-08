using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Relatorio;

namespace GestaoTarefas.Application.Interfaces;

public interface IRelatorioService
{
    Task<RespostaMetodos<RelatorioFilhoDto>> ObterPorFilhoAsync(int filhoId, int? dias);
}
