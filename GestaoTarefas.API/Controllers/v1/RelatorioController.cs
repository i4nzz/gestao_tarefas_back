using System.Net;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Controllers.v1;

/// <summary>
/// Controller do acompanhamento dos filhos, disponível apenas para o responsável.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Pai")]
public class RelatorioController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    /// <summary>
    /// Construtor da controller de relatórios, recebendo o serviço via injeção de dependência.
    /// </summary>
    /// <param name="relatorioService"></param>
    public RelatorioController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    /// <summary>
    /// Obtém o acompanhamento de um filho: tarefas por status e desempenho no questionário.
    /// </summary>
    /// <param name="filhoId">ID do filho</param>
    /// <param name="dias">Período em dias (1 a 365). Sem valor, considera todo o histórico.</param>
    [HttpGet]
    [Route("ObterPorFilho/{filhoId}")]
    public async Task<IActionResult> ObterPorFilho(int filhoId, [FromQuery] int? dias)
    {
        var relatorio = await _relatorioService.ObterPorFilhoAsync(filhoId, dias);

        if (!relatorio.Sucesso)
        {
            return StatusCode((int)relatorio.StatusCode, relatorio);
        }

        return StatusCode((int)HttpStatusCode.OK, relatorio);
    }
}
