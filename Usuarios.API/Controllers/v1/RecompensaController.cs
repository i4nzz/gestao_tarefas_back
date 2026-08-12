using System.Net;
using GestaoTarefas.Application.DTOs.Recompensa;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Controllers.v1;

/// <summary>
/// Controller para gerenciar as recompensas associadas aos filhos, permitindo criar, atualizar, deletar e resgatar recompensas, bem como consultar as recompensas disponíveis e resgatadas por cada filho.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RecompensaController : ControllerBase
{
    private readonly IRecompensaService _recompensaService;
    /// <summary>
    /// Construtor do RecompensaController, que recebe uma instância de IRecompensaService para realizar as operações relacionadas às recompensas. Essa dependência é injetada via construtor, seguindo o princípio de inversão de dependência e facilitando a testabilidade do controller.
    /// </summary>
    /// <param name="recompensaService"></param>
    public RecompensaController(IRecompensaService recompensaService)
    {
        _recompensaService = recompensaService;
    }

    /// <summary>
    /// Obtém todas as recompensas associadas a um filho específico.
    /// </summary>
    /// <param name="filhoId">ID do filho</param>
    /// <returns>Lista de recompensas do filho</returns>
    [HttpGet]
    [Route("ObterPorFilho/{filhoId}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterPorFilho(int filhoId)
    {
        var recompensas = await _recompensaService.ObterPorFilhoAsync(filhoId);

        if (!recompensas.Sucesso)
        {
            if (recompensas.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, recompensas);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, recompensas);
        }

        return StatusCode((int)HttpStatusCode.OK, recompensas);
    }

    /// <summary>
    /// Obtem os detalhes de uma recompensa específica por seu ID.
    /// </summary>
    /// <param name="id">ID da recompensa</param>
    /// <returns>Detalhes da recompensa</returns>
    [HttpGet]
    [Route("ObterPorId/{id}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var recompensa = await _recompensaService.ObterPorIdAsync(id);

        if (!recompensa.Sucesso)
        {
            if (recompensa.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, recompensa);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, recompensa);
        }

        return StatusCode((int)HttpStatusCode.OK, recompensa);
    }

    /// <summary>
    /// Criar uma nova recompensa para um filho específico. O DTO deve conter o ID do filho, a descrição da recompensa e os pontos necessários para resgatá-la.
    /// </summary>
    /// <param name="dto">Dados da recompensa</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost]
    [Route("Criar")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Criar([FromBody] CriarRecompensaDto dto)
    {
        var recompensa = await _recompensaService.CriarAsync(dto);

        if (!recompensa.Sucesso)
        {
            if (recompensa.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, recompensa);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, recompensa);
        }

        return StatusCode((int)HttpStatusCode.Created, recompensa);
    }
    /// <summary>
    /// Atualizar os detalhes de uma recompensa existente, como a descrição ou os pontos necessários.
    /// </summary>
    /// <param name="id">ID da recompensa</param>
    /// <param name="dto">Novos dados da recompensa</param>
    /// <returns>Resultado da operação</returns>
    [HttpPut]
    [Route("Atualizar/{id}")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CriarRecompensaDto dto)
    {
        var atualizado = await _recompensaService.AtualizarAsync(id, dto);

        if (!atualizado.Sucesso)
        {
            if (atualizado.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, atualizado);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, atualizado);
        }

        return StatusCode((int)HttpStatusCode.OK, atualizado);
    }

    /// <summary>
    /// Deletar uma recompensa existente, removendo-a do sistema. Essa ação deve ser confirmada para evitar exclusões acidentais.
    /// </summary>
    /// <param name="id">ID da recompensa</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete]
    [Route("Remover/{id}")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> Remover(int id)
    {
        var removido = await _recompensaService.RemoverAsync(id);

        if (!removido.Sucesso)
        {
            if (removido.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, removido);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, removido);
        }

        return StatusCode((int)HttpStatusCode.OK, removido);
    }

    /// <summary>
    /// Resgatar uma recompensa para um filho específico.
    /// </summary>
    /// <param name="filhoId">ID do filho</param>
    /// <param name="recompensaId">ID da recompensa</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost]
    [Route("Resgatar/{filhoId}/{recompensaId}")]
    [Authorize(Roles = "Filho")]
    public async Task<IActionResult> Resgatar(int filhoId, int recompensaId)
    {
        var resgatada = await _recompensaService.ResgatarAsync(filhoId, recompensaId);

        if (!resgatada.Sucesso)
        {
            if (resgatada.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, resgatada);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, resgatada);
        }

        return StatusCode((int)HttpStatusCode.OK, resgatada);
    }
    /// <summary>
    /// Retornar uma lista de todas as recompensas que um filho específico resgatou, incluindo detalhes como a data do resgate e a descrição da recompensa.
    /// </summary>
    /// <param name="filhoId">ID do filho</param>
    /// <returns>Lista de recompensas resgatadas</returns>
    [HttpGet]
    [Route("ObterResgatadas/{filhoId}")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> ObterResgatadas(int filhoId)
    {
        var resgatadas = await _recompensaService.ObterResgatadasPorFilhoAsync(filhoId);

        if (!resgatadas.Sucesso)
        {
            if (resgatadas.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, resgatadas);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, resgatadas);
        }

        return StatusCode((int)HttpStatusCode.OK, resgatadas);
    }
}