using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestaoTarefas.Application.DTOs.Tarefa;
using GestaoTarefas.Application.Interfaces;

namespace GestaoTarefas.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TarefaController : ControllerBase
{
    private readonly ITarefaService _tarefaService;

    public TarefaController(ITarefaService tarefaService)
    {
        _tarefaService = tarefaService;
    }
    /// <summary>
    /// Obter todas as tarefas.
    /// </summary>
    /// <returns>Retorna todas as tarefas cadastradas.</returns>
    [HttpGet("ObterTodas")]
    public async Task<IActionResult> ObterTodas()
    {
        var tarefas = await _tarefaService.ObterTodasAsync();

        if (!tarefas.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, tarefas.Mensagem);
        }

        return StatusCode((int)HttpStatusCode.OK, tarefas.ObjetoRetorno);
    }

    /// <summary>
    /// Obter tarefas por ID do filho.
    /// </summary>
    /// <param name="filhoId">ID do filho</param>
    /// <returns>Lista de tarefas do filho</returns>
    [HttpGet("filho/{filhoId:int}")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> ObterPorFilho(int filhoId)
    {
        var tarefas = await _tarefaService.ObterPorFilhoAsync(filhoId);

        if (!tarefas.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.NotFound, tarefas.Mensagem);
        }

        return StatusCode((int)HttpStatusCode.OK, tarefas.ObjetoRetorno);
    }

    /// <summary>
    /// Obter tarefa por ID.
    /// </summary>
    /// <param name="tarefaId">ID da tarefa</param>
    /// <returns>Retorna a tarefa correspondente ao ID fornecido</returns>
    [HttpGet("{tarefaId:int}")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> ObterPorId(int tarefaId)
    {
        var tarefa = await _tarefaService.ObterPorIdAsync(tarefaId);

        if (!tarefa.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.NotFound, tarefa.Mensagem);
        }

        return StatusCode((int)HttpStatusCode.OK, tarefa.ObjetoRetorno);
    }

    /// <summary>
    /// Criar uma nova tarefa.
    /// </summary>
    /// <param name="dto">Dados para criar a tarefa</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("CriarTarefa")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Criar([FromBody] CriarTarefaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, "Dados inválidos");
        }

        var tarefa = await _tarefaService.CriarAsync(dto);

        if (!tarefa.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, tarefa.Mensagem);
        }

        return StatusCode((int)HttpStatusCode.Created, tarefa.ObjetoRetorno);
    }

    /// <summary>
    /// Atualizar a tarefa cadastrada.
    /// </summary>
    /// <param name="tarefaId"> ID da tarefa</param>
    /// <param name="dto">Dados para atualizar a tarefa</param>
    /// <returns>Resultado da operação</returns>
    [HttpPut("{tarefaId:int}")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Atualizar(int tarefaId, [FromBody] CriarTarefaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, "Dados inválidos");
        }

        var existe = await _tarefaService.ObterPorIdAsync(tarefaId);

        if (!existe.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.NotFound, existe.Mensagem);
        }

        var resultado = await _tarefaService.AtualizarAsync(tarefaId, dto);

        return StatusCode((int)HttpStatusCode.OK, resultado.Mensagem);
    }
    /// <summary>
    /// Remover a tarefa cadastrada.
    /// </summary>
    /// <param name="tarefaId">ID da tarefa</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{tarefaId:int}")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Remover(int tarefaId)
    {
        var existe = await _tarefaService.ObterPorIdAsync(tarefaId);

        if (!existe.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.NotFound, existe.Mensagem);
        }

        var removido = await _tarefaService.RemoverAsync(tarefaId);

        return StatusCode((int)HttpStatusCode.OK, removido.Mensagem);
    }
}