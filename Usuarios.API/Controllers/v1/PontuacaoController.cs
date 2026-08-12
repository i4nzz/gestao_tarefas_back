using System.Net;
using GestaoTarefas.Application.DTOs.Pontuacao;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Controllers.v1;

/// <summary>
/// Controller para gerenciar as pontuacoes
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PontuacaoController : ControllerBase
{
    private readonly IPontuacaoService _pontuacaoService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pontuacaoService"></param>
    public PontuacaoController(IPontuacaoService pontuacaoService)
    {
        _pontuacaoService = pontuacaoService;
    }

    /// <summary>
    /// Obter as pontuações de um filho específico.
    /// </summary>
    /// <param name="filhoId">ID do filho</param>
    /// <returns>Lista de pontuações do filho</returns>
    [HttpGet]
    [Route("ObterPorFilho/{filhoId}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterPorFilho(int filhoId)
    {
        var pontuacoes = await _pontuacaoService.ObterPorFilhoAsync(filhoId);

        if (!pontuacoes.Sucesso)
        {
            if (pontuacoes.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, pontuacoes);
            }

            return StatusCode((int)HttpStatusCode.NoContent, pontuacoes);
        }

        return StatusCode((int)HttpStatusCode.OK, pontuacoes.ObjetoRetorno);
    }

    /// <summary>
    /// Obter o total de pontos acumulados por um filho específico.
    /// </summary>
    /// <param name="filhoId">ID do filho</param>
    /// <returns>Total de pontos do filho</returns>
    [HttpGet]
    [Route("ObterTotal/{filhoId}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterTotal(int filhoId)
    {
        var total = await _pontuacaoService.ObterTotalPontosAsync(filhoId);

        if (!total.Sucesso)
        {
            if (total.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, total.Mensagem);
            }

            return StatusCode((int)HttpStatusCode.NoContent, total.Mensagem);
        }

        return StatusCode((int)HttpStatusCode.OK, total.ObjetoRetorno);
    }

    /// <summary>
    /// Adicionar uma nova pontuação para um filho.
    /// </summary>
    /// <param name="dto">Dados da pontuação</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost]
    [Route("Adicionar")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Adicionar([FromBody] CriarPontuacaoDto dto)
    {
        var pontuacao = await _pontuacaoService.AdicionarAsync(dto);

        if (!pontuacao.Sucesso)
        {
            if (pontuacao.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, pontuacao.Mensagem);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, pontuacao.Mensagem);
        }

        return StatusCode((int)HttpStatusCode.Created, pontuacao.ObjetoRetorno);
    }
}