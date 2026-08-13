using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.CategoriaFinanceira;

namespace GestaoTarefas.Application.Interfaces;

public interface ICategoriaFinanceiraService
{
    Task<RespostaMetodos<IEnumerable<RetornoCategoriaFinanceiraDto>>> ObterTodasAsync();
    Task<RespostaMetodos<RetornoCategoriaFinanceiraDto?>> ObterPorIdAsync(int id);
    Task<RespostaMetodos<RetornoCategoriaFinanceiraDto>> CriarAsync(CriarCategoriaFinanceiraDto dto);
    Task<RespostaMetodos<RetornoCategoriaFinanceiraDto>> AtualizarAsync(int id, CriarCategoriaFinanceiraDto dto);
    Task<RespostaMetodos<RetornoCategoriaFinanceiraDto>> RemoverAsync(int id);
}
