using System.Net;
using GestaoTarefas.Application.DTOs.CategoriaFinanceira;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Controllers.v1;

/// <summary>
/// Controller para gerenciar as categorias financeiras usadas para classificar os registros de gastos da mesada.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CategoriaFinanceiraController : ControllerBase
{
    private readonly ICategoriaFinanceiraService _categoriaFinanceiraService;

    public CategoriaFinanceiraController(ICategoriaFinanceiraService categoriaFinanceiraService)
    {
        _categoriaFinanceiraService = categoriaFinanceiraService;
    }

    /// <summary>
    /// Obtém todas as categorias financeiras cadastradas.
    /// </summary>
    [HttpGet]
    [Route("ObterTodas")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterTodas()
    {
        var categorias = await _categoriaFinanceiraService.ObterTodasAsync();

        if (!categorias.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, categorias);
        }

        return StatusCode((int)HttpStatusCode.OK, categorias);
    }

    /// <summary>
    /// Obtém uma categoria financeira específica por seu ID.
    /// </summary>
    [HttpGet]
    [Route("ObterPorId/{id}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var categoria = await _categoriaFinanceiraService.ObterPorIdAsync(id);

        if (!categoria.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, categoria);
        }

        return StatusCode((int)HttpStatusCode.OK, categoria);
    }

    /// <summary>
    /// Cria uma nova categoria financeira.
    /// </summary>
    [HttpPost]
    [Route("Criar")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Criar([FromBody] CriarCategoriaFinanceiraDto dto)
    {
        var categoria = await _categoriaFinanceiraService.CriarAsync(dto);

        if (!categoria.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, categoria);
        }

        return StatusCode((int)HttpStatusCode.Created, categoria);
    }

    /// <summary>
    /// Atualiza uma categoria financeira existente.
    /// </summary>
    [HttpPut]
    [Route("Atualizar/{id}")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CriarCategoriaFinanceiraDto dto)
    {
        var atualizado = await _categoriaFinanceiraService.AtualizarAsync(id, dto);

        if (!atualizado.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, atualizado);
        }

        return StatusCode((int)HttpStatusCode.OK, atualizado);
    }

    /// <summary>
    /// Remove uma categoria financeira existente.
    /// </summary>
    [HttpDelete]
    [Route("Remover/{id}")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Remover(int id)
    {
        var removido = await _categoriaFinanceiraService.RemoverAsync(id);

        if (!removido.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, removido);
        }

        return StatusCode((int)HttpStatusCode.OK, removido);
    }
}
