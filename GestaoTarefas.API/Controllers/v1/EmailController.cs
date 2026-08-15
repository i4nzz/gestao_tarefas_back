using System.Net;
using GestaoTarefas.Application.DTOs.Email;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.API.Controllers.v1;
/// <summary>
/// Controller para envio de emails, responsável por gerenciar as operações relacionadas ao envio de e-mails, como e-mails de confirmação, notificações e outros tipos de comunicação por e-mail dentro do sistema. Essa controller pode incluir métodos para enviar e-mails simples, e-mails com templates personalizados, e-mails de recuperação de senha, entre outros, utilizando os serviços de e-mail configurados na aplicação.
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Pai")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailController> _logger;
    private const string MensagemModelState = "Erro validação dos dados de entrada.";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailService"></param>
    /// <param name="logger"></param>
    public EmailController(IEmailService emailService, ILogger<EmailController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }
    /// <summary>
    /// Enviar email
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("enviar")]
    [ProducesResponseType(typeof(EmailResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Enviar([FromBody] SendSimpleEmailDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode((int)HttpStatusCode.UnprocessableEntity, MensagemModelState);
        }

        var request = new SendEmailRequest
        {
            ListaDestinatarios = [dto.To],
            Assunto = dto.Subject,
        };

        if (dto.IsHtml)
        {
            request.HtmlBody = dto.Body;
        }
        else
        {
            request.CorpoTexto = dto.Body;
        }

        var result = await _emailService.EnviarAsync(request, cancellationToken);

        if (result.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.OK, result.ObjetoRetorno);
        }

        return StatusCode((int)HttpStatusCode.BadRequest, result.Mensagem);
    }
    /// <summary>
    /// Enviar email de confirmação.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    [HttpPost("confirmation")]
    [ProducesResponseType(typeof(EmailResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EnviarConfirmacao([FromBody] SendConfirmationEmailDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _emailService.EnviarEmailConfirmacaoAsync(dto.ToEmail, dto.UserName, dto.Link, cancellationToken);

        if (result.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.OK, result.ObjetoRetorno);
        }

        return StatusCode((int)HttpStatusCode.BadRequest, result.Mensagem);
    }

    /// <summary>
    /// Envia e-mail de recuperação de senha com template HTML.
    /// </summary>
    [HttpPost("password-reset")]
    [ProducesResponseType(typeof(EmailResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendPasswordReset([FromBody] SendConfirmationEmailDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode((int)HttpStatusCode.UnprocessableContent, MensagemModelState);
        }

        var result = await _emailService.EnviarEmailResetSenhaAsync(dto.ToEmail, dto.UserName, dto.Link, cancellationToken);

        if (result.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.OK, result.ObjetoRetorno);
        }

        return StatusCode((int)HttpStatusCode.BadRequest, result.Mensagem);
    }

    /// <summary>
    /// Envia uma notificação genérica do sistema.
    /// </summary>
    [HttpPost("notification")]
    [ProducesResponseType(typeof(EmailResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode((int)HttpStatusCode.UnprocessableEntity, MensagemModelState);
        }

        var result = await _emailService.EnviarNotificacaoSistemaAsync(dto.ToEmail, dto.Subject, dto.Message, cancellationToken);

        if (result.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.OK, result.ObjetoRetorno);
        }

        return StatusCode((int)HttpStatusCode.BadRequest, result.Mensagem);
    }
}
