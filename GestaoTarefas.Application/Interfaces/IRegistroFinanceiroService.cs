using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.RegistroFinanceiro;

namespace GestaoTarefas.Application.Interfaces;

public interface IRegistroFinanceiroService
{
    Task<RespostaMetodos<IEnumerable<RetornoRegistroFinanceiroDto>>> ObterPorFilhoAsync(int filhoId);
    Task<RespostaMetodos<RetornoRegistroFinanceiroDto>> CriarAsync(CriarRegistroFinanceiroDto dto);
}
