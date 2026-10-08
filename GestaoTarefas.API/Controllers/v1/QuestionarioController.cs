using System.Net;
using GestaoTarefas.Application.DTOs.Questionario;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Controllers.v1;

/// <summary>
/// Controller do questionário gamificado (módulo de jogos).
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class QuestionarioController : ControllerBase
{
    private readonly IQuestionarioService _questionarioService;

    /// <summary>
    /// Construtor da controller do questionário, recebendo o serviço via injeção de dependência.
    /// </summary>
    /// <param name="questionarioService"></param>
    public QuestionarioController(IQuestionarioService questionarioService)
    {
        _questionarioService = questionarioService;
    }

    /// <summary>
    /// Inicia uma partida: sorteia perguntas aleatórias do banco de perguntas, sem a resposta correta.
    /// </summary>
    /// <param name="quantidade">Quantidade de perguntas (padrão 10, máximo 20)</param>
    [HttpGet]
    [Route("Iniciar")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> Iniciar([FromQuery] int? quantidade)
    {
        var perguntas = await _questionarioService.IniciarAsync(quantidade);

        if (!perguntas.Sucesso)
        {
            return StatusCode((int)perguntas.StatusCode, perguntas);
        }

        return StatusCode((int)HttpStatusCode.OK, perguntas);
    }

    /// <summary>
    /// Valida a alternativa escolhida e devolve se acertou, a explicação e os pontos ganhos.
    /// </summary>
    /// <param name="dto">Pergunta e índice da alternativa escolhida</param>
    [HttpPost]
    [Route("Responder")]
    [Authorize(Roles = "Pai,Filho")]
    public async Task<IActionResult> Responder([FromBody] ResponderPerguntaDto dto)
    {
        var resposta = await _questionarioService.ResponderAsync(dto);

        if (!resposta.Sucesso)
        {
            return StatusCode((int)resposta.StatusCode, resposta);
        }

        return StatusCode((int)HttpStatusCode.OK, resposta);
    }
}
