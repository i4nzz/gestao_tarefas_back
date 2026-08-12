using System.Net;
using GestaoTarefas.Application.DTOs.Mesada;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Controllers.v1;

/// <summary>
/// Controller para gerenciar as mesadas registradas para os filhos.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MesadaController : ControllerBase
{
    private readonly IMesadaService _mesadaService;

    public MesadaController(IMesadaService mesadaService)
    {
        _mesadaService = mesadaService;
    }

    /// <summary>
    /// Obtém as mesadas registradas para um filho específico.
    /// </summary>
    [HttpGet]
    [Route("ObterPorFilho/{filhoId}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterPorFilho(int filhoId)
    {
        var mesadas = await _mesadaService.ObterPorFilhoAsync(filhoId);

        if (!mesadas.Sucesso)
        {
            if (mesadas.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, mesadas);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, mesadas);
        }

        return StatusCode((int)HttpStatusCode.OK, mesadas);
    }

    /// <summary>
    /// Registra uma nova mesada para um filho.
    /// </summary>
    [HttpPost]
    [Route("Criar")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Criar([FromBody] CriarMesadaDto dto)
    {
        var mesada = await _mesadaService.CriarAsync(dto);

        if (!mesada.Sucesso)
        {
            if (mesada.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, mesada);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, mesada);
        }

        return StatusCode((int)HttpStatusCode.Created, mesada);
    }
}
