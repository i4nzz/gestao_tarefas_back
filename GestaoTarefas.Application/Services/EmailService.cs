using System.Net;
using System.Text.RegularExpressions;
using GestaoTarefas.Application.DTOs.Email;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.Configuration;
using GestaoTarefas.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

namespace GestaoTarefas.Application.Services;

public class EmailService : IEmailService
{

    private readonly IResend _resend;
    private readonly ResendOptions _resendOptions;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IResend resend,
        IOptions<ResendOptions> options,
        ILogger<EmailService> logger)
    {
        _resend = resend;
        _resendOptions = options.Value;
        _logger = logger;
    }
    public async Task<RespostaMetodos<EmailResult>> EnviarAsync(SendEmailRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidarRequisicao(request);

            var message = new EmailMessage
            {
                From = request.Remetente ?? _resendOptions.FormattedFrom,
                To = EmailAddressList.From(request.ListaDestinatarios),
                Subject = request.Assunto,
            };

            if (!string.IsNullOrWhiteSpace(request.HtmlBody))
            {
                message.HtmlBody = request.HtmlBody;
            }

            else if (!string.IsNullOrWhiteSpace(request.CorpoTexto))
            {
                message.TextBody = request.CorpoTexto;
            }

            else
            {
                throw new ArgumentException("É obrigatório fornecer HtmlBody ou TextBody.");
            }

            if (request.DestinatariosEmCopia is { Count: > 0 })
            {
                message.Cc = EmailAddressList.From(request.DestinatariosEmCopia);
            }

            if (request.DestinatariosEmCopiaOculta is { Count: > 0 })
            {
                message.Bcc = EmailAddressList.From(request.DestinatariosEmCopiaOculta);
            }

            if (request.Tags is { Count: > 0 })
            {
                message.Tags = request.Tags.Select(t => new EmailTag { Name = t.Key, Value = t.Value }).ToList();
            }

            _logger.LogInformation("Enviando e-mail | Para: {To} | Assunto: {Subject}", string.Join(", ", request.ListaDestinatarios), request.Assunto);

            var response = await _resend.EmailSendAsync(message, cancellationToken);

            _logger.LogInformation("E-mail enviado com sucesso | ID: {EmailId}", response.Content);

            var respostaEmail = EmailResult.Ok(response.Content.ToString());

            return new RespostaMetodos<EmailResult>
            {
                Sucesso = true,
                StatusCode = HttpStatusCode.OK,
                ObjetoRetorno = respostaEmail,
                Mensagem = "E-mail enviado com sucesso."
            };
        }
        catch (ResendException ex)
        {
            _logger.LogError(ex, "Erro do Resend ao enviar e-mail para {To}: {Error}", string.Join(", ", request.ListaDestinatarios), ex.Message);

            return new RespostaMetodos<EmailResult>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.BadRequest,
                ObjetoRetorno = EmailResult.Fail($"Erro do provedor: {ex.Message}"),
                Mensagem = "Falha ao enviar e-mail."
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos na requisição de e-mail: {Error}", ex.Message);
            return new RespostaMetodos<EmailResult>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.BadRequest,
                ObjetoRetorno = EmailResult.Fail($"Dados inválidos: {ex.Message}"),
                Mensagem = "Falha ao enviar e-mail devido a dados inválidos."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao enviar e-mail para {To}", string.Join(", ", request.ListaDestinatarios));
            return new RespostaMetodos<EmailResult>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ObjetoRetorno = EmailResult.Fail("Erro inesperado ao enviar e-mail."),
                Mensagem = "Falha ao enviar e-mail devido a um erro inesperado."
            };
        }
    }

    public async Task<RespostaMetodos<EmailResult>> EnviarEmailConfirmacaoAsync(string toEmail, string userName, string confirmationLink, CancellationToken cancellationToken = default)
    {
        var html = $"""
            <!DOCTYPE html>
            <html lang="pt-BR">

            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Confirme seu e-mail</title>
            </head>

            <body style="margin:0;padding:0;background-color:#f4f7fb;font-family:Arial,Helvetica,sans-serif;">

                <table width="100%" cellpadding="0" cellspacing="0" border="0" style="background:#f4f7fb;padding:40px 20px;">
                    <tr>
                        <td align="center">

                            <table width="600" cellpadding="0" cellspacing="0" border="0"
                                style="background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 6px 18px rgba(0,0,0,0.08);">

                                <tr>
                                    <td align="center"
                                        style="background:linear-gradient(135deg,#1E3A5F,#2D8CF0);padding:36px;">

                                        <div style="font-size:48px;margin-bottom:10px;">📧</div>

                                        <h1 style="margin:0;color:#ffffff;font-size:30px;font-weight:bold;">
                                            Task Kids - Confirmação de e-mail
                                        </h1>

                                        <p style="margin-top:10px;color:#dbeafe;font-size:15px;">
                                            Educação financeira e tarefas para toda a família
                                        </p>

                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:45px 40px;">

                                        <h2 style="margin-top:0;color:#1E3A5F;font-size:28px;">
                                            Olá, {userName}! 👋
                                        </h2>

                                        <p style="font-size:16px;color:#475569;line-height:1.8;margin-bottom:24px;">
                                            Seja muito bem-vindo ao
                                            <strong>Task Kids</strong>!
                                        </p>

                                        <p style="font-size:16px;color:#475569;line-height:1.8;margin-bottom:30px;">
                                            Estamos felizes por ter você conosco.
                                            Para concluir seu cadastro e proteger sua conta,
                                            basta confirmar seu endereço de e-mail clicando
                                            no botão abaixo.
                                        </p>

                                        <div style="text-align:center;margin:40px 0;">

                                            <a href="{confirmationLink}"
                                                style="
                                                    background:#2D8CF0;
                                                    color:#ffffff;
                                                    text-decoration:none;
                                                    padding:16px 36px;
                                                    border-radius:10px;
                                                    display:inline-block;
                                                    font-size:17px;
                                                    font-weight:bold;
                                                    box-shadow:0 4px 12px rgba(45,140,240,.3);
                                                ">
                                                ✅ Confirmar meu e-mail
                                            </a>

                                        </div>


                                        <p style="
                                            background:#f8fafc;
                                            padding:14px;
                                            border-radius:8px;
                                            word-break:break-all;
                                            font-size:13px;
                                            color:#2563eb;
                                            border:1px solid #e2e8f0;
                                        ">
                                            {confirmationLink}
                                        </p>

                                        <!-- Aviso -->
                                        <table width="100%" cellpadding="0" cellspacing="0"
                                            style="margin-top:30px;background:#eff6ff;border-left:4px solid #2D8CF0;border-radius:8px;">
                                            <tr>
                                                <td style="padding:18px;">

                                                    <strong style="color:#1E3A5F;">
                                                        🔒 Segurança
                                                    </strong>

                                                    <p style="margin:10px 0 0;color:#475569;font-size:14px;line-height:1.7;">
                                                        Este link permanecerá válido por
                                                        <strong>24 horas</strong>.
                                                        Caso você não tenha criado uma conta no
                                                        Task Kids, basta ignorar este e-mail.
                                                    </p>

                                                </td>
                                            </tr>
                                        </table>

                                    </td>
                                </tr>
                                <tr>
                                    <td align="center"
                                        style="padding:28px;background:#f8fafc;border-top:1px solid #e2e8f0;">

                                        <p style="margin:0;font-size:13px;color:#64748b;">
                                            © 2026 <strong>TaskiDs</strong>
                                        </p>

                                        <p style="margin-top:10px;font-size:12px;color:#94a3b8;">
                                            Este é um e-mail automático. Não responda esta mensagem.
                                        </p>

                                    </td>
                                </tr>

                            </table>

                        </td>
                    </tr>
                </table>

            </body>

            </html>
            """;

        var respostaEnviarEmail = await EnviarAsync(new SendEmailRequest
        {
            ListaDestinatarios = [toEmail],
            Assunto = "Confirme seu e-mail",
            HtmlBody = html,
            Tags = new Dictionary<string, string>
            {
                ["type"] = "confirmation",
                ["version"] = "v1"
            }
        }, cancellationToken);

        if (respostaEnviarEmail.Sucesso)
        {
            return new RespostaMetodos<EmailResult>
            {
                Sucesso = true,
                StatusCode = HttpStatusCode.OK,
                ObjetoRetorno = respostaEnviarEmail.ObjetoRetorno,
                Mensagem = "E-mail de confirmação enviado com sucesso."
            };
        }
        return new RespostaMetodos<EmailResult>
        {
            Sucesso = false,
            StatusCode = respostaEnviarEmail.StatusCode,
            ObjetoRetorno = respostaEnviarEmail.ObjetoRetorno,
            Mensagem = "Falha ao enviar e-mail de confirmação."
        };

    }

    public async Task<RespostaMetodos<EmailResult>> EnviarEmailResetSenhaAsync(string toEmail, string userName, string resetLink, CancellationToken cancellationToken = default)
    {
        var html = $"""
            <!DOCTYPE html>
            <html lang="pt-BR">

            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Redefinição de Senha</title>
            </head>

            <body style="margin:0;padding:0;background-color:#f4f7fb;font-family:Arial,Helvetica,sans-serif;">

                <table width="100%" cellpadding="0" cellspacing="0" border="0" style="background:#f4f7fb;padding:40px 20px;">
                    <tr>
                        <td align="center">

                            <table width="600" cellpadding="0" cellspacing="0" border="0"
                                style="background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 6px 18px rgba(0,0,0,0.08);">

                                <!-- Cabeçalho -->
                                <tr>
                                    <td align="center"
                                        style="background:linear-gradient(135deg,#1E3A5F,#2D8CF0);padding:36px;">

                                        <div style="font-size:48px;margin-bottom:10px;">🔐</div>

                                        <h1 style="margin:0;color:#ffffff;font-size:30px;font-weight:bold;">
                                            Task Kids
                                        </h1>

                                        <p style="margin-top:10px;color:#dbeafe;font-size:15px;">
                                            Segurança da sua conta
                                        </p>

                                    </td>
                                </tr>

                                <!-- Conteúdo -->
                                <tr>
                                    <td style="padding:45px 40px;">

                                        <h2 style="margin-top:0;color:#1E3A5F;font-size:28px;">
                                            Redefinição de senha
                                        </h2>

                                        <p style="font-size:16px;color:#475569;line-height:1.8;margin-bottom:24px;">
                                            Olá, <strong>{userName}</strong>! 👋
                                        </p>

                                        <p style="font-size:16px;color:#475569;line-height:1.8;margin-bottom:30px;">
                                            Recebemos uma solicitação para redefinir a senha da sua conta no
                                            <strong>Task Kids</strong>.
                                            Caso tenha sido você, clique no botão abaixo para criar uma nova senha.
                                        </p>

                                        <!-- Botão -->
                                        <div style="text-align:center;margin:40px 0;">

                                            <a href="{resetLink}"
                                                style="
                                                    background:#EA580C;
                                                    color:#ffffff;
                                                    text-decoration:none;
                                                    padding:16px 36px;
                                                    border-radius:10px;
                                                    display:inline-block;
                                                    font-size:17px;
                                                    font-weight:bold;
                                                    box-shadow:0 4px 12px rgba(234,88,12,.35);
                                                ">
                                                🔑 Redefinir minha senha
                                            </a>

                                        </div>

                                        <p style="
                                            background:#f8fafc;
                                            padding:14px;
                                            border-radius:8px;
                                            word-break:break-all;
                                            font-size:13px;
                                            color:#2563eb;
                                            border:1px solid #e2e8f0;
                                        ">
                                            {resetLink}
                                        </p>

                                        <!-- Aviso -->
                                        <table width="100%" cellpadding="0" cellspacing="0"
                                            style="margin-top:30px;background:#FFF7ED;border-left:4px solid #EA580C;border-radius:8px;">
                                            <tr>
                                                <td style="padding:18px;">

                                                    <strong style="color:#9A3412;">
                                                        🛡️ Aviso de segurança
                                                    </strong>

                                                    <p style="margin:10px 0 0;color:#7C2D12;font-size:14px;line-height:1.7;">
                                                        Este link permanecerá válido por
                                                        <strong>1 hora</strong>.
                                                        Caso você não tenha solicitado a redefinição da senha,
                                                        ignore este e-mail. Sua senha atual continuará a mesma.
                                                    </p>

                                                </td>
                                            </tr>
                                        </table>

                                    </td>
                                </tr>

                                <!-- Rodapé -->
                                <tr>
                                    <td align="center"
                                        style="padding:28px;background:#f8fafc;border-top:1px solid #e2e8f0;">

                                        <p style="margin:0;font-size:13px;color:#64748b;">
                                            © 2026 <strong>Task Kids</strong>
                                        </p>

                                        <p style="margin-top:10px;font-size:12px;color:#94a3b8;">
                                            Este é um e-mail automático. Não responda esta mensagem.
                                        </p>

                                    </td>
                                </tr>

                            </table>

                        </td>
                    </tr>
                </table>

            </body>

            </html>
            """;

        var respostaEnviarEmail = await EnviarAsync(new SendEmailRequest
        {
            ListaDestinatarios = [toEmail],
            Assunto = "Redefinição de senha",
            HtmlBody = html,
            Tags = new Dictionary<string, string>
            {
                ["type"] = "password-reset",
                ["version"] = "v1"
            }
        }, cancellationToken);

        if (respostaEnviarEmail.Sucesso)
        {
            return new RespostaMetodos<EmailResult>
            {
                Sucesso = true,
                StatusCode = HttpStatusCode.OK,
                ObjetoRetorno = respostaEnviarEmail.ObjetoRetorno,
                Mensagem = "E-mail de redefinição de senha enviado com sucesso."
            };
        }

        return new RespostaMetodos<EmailResult>
        {
            Sucesso = false,
            StatusCode = respostaEnviarEmail.StatusCode,
            ObjetoRetorno = respostaEnviarEmail.ObjetoRetorno,
            Mensagem = "Falha ao enviar e-mail de redefinição de senha."
        };

    }

    public async Task<RespostaMetodos<EmailResult>> EnviarNotificacaoSistemaAsync(string toEmail, string subject, string message, CancellationToken cancellationToken = default)
    {
        var html = $"""
            <!DOCTYPE html>
            <html lang="pt-BR">
            <head><meta charset="UTF-8"></head>
            <body style="margin:0;padding:0;background:#f1f5f9;font-family:Arial,sans-serif;">
              <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                  <td align="center" style="padding:40px 16px;">
                    <table width="600" cellpadding="0" cellspacing="0"style="background:#ffffff;border-radius:12px;overflow:hidden;">

                      <tr>
                        <td style="background:#1E3A5F;padding:24px 32px;">
                          <h2 style="color:#ffffff;margin:0;font-size:18px;">
                            🔔 Notificação
                          </h2>
                        </td>
                      </tr>

                      <tr>
                        <td style="padding:32px;">
                          <p style="color:#1e293b;font-size:16px;line-height:1.6;margin:0;">
                            {message}
                          </p>
                        </td>
                      </tr>

                      <tr>
                        <td style="background:#f8fafc;padding:16px 32px;border-top:1px solid #e2e8f0;text-align:center;">
                        </td>
                      </tr>

                    </table>
                  </td>
                </tr>
              </table>
            </body>
            </html>
            """;

        var respostaEnviarEmail = await EnviarAsync(new SendEmailRequest
        {
            ListaDestinatarios = [toEmail],
            Assunto = subject,
            HtmlBody = html,
            Tags = new Dictionary<string, string>
            {
                ["type"] = "system-notification"
            }
        }, cancellationToken);

        if (respostaEnviarEmail.Sucesso)
        {
            return new RespostaMetodos<EmailResult>
            {
                StatusCode = HttpStatusCode.OK,
                ObjetoRetorno = respostaEnviarEmail.ObjetoRetorno,
                Mensagem = "Notificação enviada com sucesso.",
            };
        }

        return new RespostaMetodos<EmailResult>
        {
            Sucesso = false,
            StatusCode = respostaEnviarEmail.StatusCode,
            ObjetoRetorno = respostaEnviarEmail.ObjetoRetorno,
            Mensagem = "Falha ao enviar notificação."
        };
    }

    private static void ValidarRequisicao(SendEmailRequest request)
    {
        if (request.ListaDestinatarios is not { Count: > 0 })
        {
            throw new ArgumentException("Informe pelo menos um destinatário.", nameof(request.ListaDestinatarios));
        }

        if (string.IsNullOrWhiteSpace(request.Assunto))
        {
            throw new ArgumentException("O assunto é obrigatório.", nameof(request.Assunto));
        }

        if (string.IsNullOrWhiteSpace(request.CorpoTexto) && string.IsNullOrWhiteSpace(request.HtmlBody))
        {
            throw new ArgumentException("Informe CorpoTexto ou HtmlBody.");
        }

        foreach (var email in request.ListaDestinatarios)
        {
            if (!ValidarEmail(email))
            {
                throw new ArgumentException($"Endereço de e-mail inválido: '{email}'", nameof(request.ListaDestinatarios));
            }
        }

        if (request.DestinatariosEmCopia is not null)
        {
            foreach (var email in request.DestinatariosEmCopia)
            {
                if (!ValidarEmail(email))
                {
                    throw new ArgumentException($"Endereço de e-mail inválido em cópia: '{email}'", nameof(request.DestinatariosEmCopia));
                }
            }
        }

        if (request.DestinatariosEmCopiaOculta is not null)
        {
            foreach (var email in request.DestinatariosEmCopiaOculta)
            {
                if (!ValidarEmail(email))
                {
                    throw new ArgumentException($"Endereço de e-mail inválido em cópia oculta: '{email}'", nameof(request.DestinatariosEmCopiaOculta));
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Remetente) && !ValidarEmail(request.Remetente))
        {
            throw new ArgumentException($"Endereço de remetente inválido: '{request.Remetente}'", nameof(request.Remetente));
        }
    }

    private static bool ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
    }
}
