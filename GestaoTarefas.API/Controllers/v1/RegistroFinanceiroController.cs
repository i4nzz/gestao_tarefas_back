using System.Net;
using GestaoTarefas.Application.DTOs.RegistroFinanceiro;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Controllers.v1;

/// <summary>
/// Controller para gerenciar os registros financeiros (gastos) associados à mesada de um filho.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RegistroFinanceiroController : ControllerBase
{
    private readonly IRegistroFinanceiroService _registroFinanceiroService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="registroFinanceiroService"></param>
    public RegistroFinanceiroController(IRegistroFinanceiroService registroFinanceiroService)
    {
        _registroFinanceiroService = registroFinanceiroService;
    }

    /// <summary>
    /// Obtém os registros financeiros de um filho específico.
    /// </summary>
    [HttpGet]
    [Route("ObterPorFilho/{filhoId}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterPorFilho(int filhoId)
    {
        var registros = await _registroFinanceiroService.ObterPorFilhoAsync(filhoId);

        if (!registros.Sucesso)
        {
            if (registros.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, registros);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, registros);
        }

        return StatusCode((int)HttpStatusCode.OK, registros);
    }

    /// <summary>
    /// Obtém o resumo financeiro de um filho, com total de mesadas, total gasto, saldo e o detalhamento por categoria (para gráficos/tabelas).
    /// </summary>
    [HttpGet]
    [Route("ObterResumo/{filhoId}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterResumo(int filhoId)
    {
        var resumo = await _registroFinanceiroService.ObterResumoPorFilhoAsync(filhoId);

        if (!resumo.Sucesso)
        {
            if (resumo.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, resumo);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, resumo);
        }

        return StatusCode((int)HttpStatusCode.OK, resumo);
    }

    /// <summary>
    /// Cria um novo registro financeiro (gasto) vinculado a uma mesada e categoria.
    /// </summary>
    [HttpPost]
    [Route("Criar")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> Criar([FromBody] CriarRegistroFinanceiroDto dto)
    {
        var registro = await _registroFinanceiroService.CriarAsync(dto);

        if (!registro.Sucesso)
        {
            if (registro.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, registro);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, registro);
        }

        return StatusCode((int)HttpStatusCode.Created, registro);
    }
}
