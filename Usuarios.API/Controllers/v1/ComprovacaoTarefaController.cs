using System.Net;
using GestaoTarefas.Application.DTOs.Recompensa;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Controllers.v1;

/// <summary>
/// Controller responsável por gerenciar as comprovações de tarefas, permitindo que os filhos enviem comprovações e os pais validem essas comprovações (aprovando ou rejeitando).
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ComprovacaoTarefaController : ControllerBase
{
    private readonly IComprovacaoService _comprovacaoTarefaService;
    /// <summary>
    /// Construtor da controller de comprovação de tarefa, recebendo a dependência do serviço de comprovação via injeção de dependência.
    /// </summary>
    /// <param name="comprovacaoTarefaService"></param>
    public ComprovacaoTarefaController(IComprovacaoService comprovacaoTarefaService)
    {
        _comprovacaoTarefaService = comprovacaoTarefaService;
    }

    /// <summary>
    /// Obter comprovações de uma tarefa específica
    /// </summary>
    /// <param name="tarefaId">ID da tarefa</param>
    /// <returns>Lista de comprovações da tarefa</returns>
    [HttpGet("tarefa/{tarefaId:int}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterPorTarefa(int tarefaId)
    {
        var resultado = await _comprovacaoTarefaService.ObterPorTarefaAsync(tarefaId);

        if (!resultado.Sucesso)
        {
            var statusCode = resultado.StatusCode != 0 ? (int)resultado.StatusCode : (int)HttpStatusCode.BadRequest;
            return StatusCode(statusCode, resultado.Mensagem);
        }

        return StatusCode((int)HttpStatusCode.OK, resultado.ObjetoRetorno);
    }
    /// <summary>
    /// Obter comprovações de uma tarefa específica pelo seu ID
    /// </summary>
    /// <param name="id">ID da comprovação</param>
    /// <returns>Comprovação da tarefa</returns>

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var resultado = await _comprovacaoTarefaService.ObterPorIdAsync(id);

        if (!resultado.Sucesso)
        {
            var statusCode = resultado.StatusCode != 0 ? (int)resultado.StatusCode : (int)HttpStatusCode.BadRequest;
            return StatusCode(statusCode, resultado.Mensagem);
        }

        return StatusCode((int)HttpStatusCode.OK, resultado.ObjetoRetorno);
    }

    /// <summary>
    /// Enviar comprovação de tarefa para validação
    /// </summary>
    /// <param name="dto">Dados da comprovação</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("enviar")]
    [Authorize(Roles = "Filho,Pai")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Enviar([FromForm] CriarComprovacaoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode((int)HttpStatusCode.BadRequest);
        }

        var resultado = await _comprovacaoTarefaService.EnviarAsync(dto);

        if (!resultado.Sucesso)
        {
            var statusCode = resultado.StatusCode != 0 ? (int)resultado.StatusCode : (int)HttpStatusCode.BadRequest;
            return StatusCode(statusCode, resultado.Mensagem);
        }

        return StatusCode((int)HttpStatusCode.OK, resultado.ObjetoRetorno);
    }

    /// <summary>
    /// Obtém a foto de uma comprovação. Acesso restrito ao Filho dono da tarefa ou ao Pai vinculado a ele.
    /// </summary>
    [HttpGet("{id:int}/foto")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterFoto(int id)
    {
        var resultado = await _comprovacaoTarefaService.ObterFotoAsync(id);

        if (!resultado.Sucesso || resultado.ObjetoRetorno == null)
        {
            var statusCode = resultado.StatusCode != 0 ? (int)resultado.StatusCode : (int)HttpStatusCode.BadRequest;
            return StatusCode(statusCode, resultado.Mensagem);
        }

        var (conteudo, contentType) = resultado.ObjetoRetorno.Value;
        return File(conteudo, contentType);
    }


    /// <summary>
    /// Validar comprovação de tarefa (aprovando ou rejeitando)
    /// </summary>
    /// <param name="id">ID da comprovação</param>
    /// <param name="aprovar">Indica se a comprovação deve ser aprovada (true) ou rejeitada (false)</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("validar/{id:int}")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Validar(int id, bool aprovar)
    {
        var resultado = await _comprovacaoTarefaService.ValidarAsync(id, aprovar);

        if (!resultado.Sucesso)
        {
            var statusCode = resultado.StatusCode != 0 ? (int)resultado.StatusCode : (int)HttpStatusCode.BadRequest;
            return StatusCode(statusCode, resultado.Mensagem);
        }

        return StatusCode((int)HttpStatusCode.OK, resultado.Mensagem);
    }
}