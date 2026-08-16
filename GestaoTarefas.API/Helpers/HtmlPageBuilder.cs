using System.Net;

namespace GestaoTarefas.Helpers;

public static class HtmlPageBuilder
{
    public static string BuildMessagePage(bool sucesso, string titulo, string mensagem)
    {
        var cor = sucesso ? "#16A34A" : "#DC2626";
        var icone = sucesso ? "✅" : "⚠️";

        return $"""
            <!DOCTYPE html>
            <html lang="pt-BR">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>{WebUtility.HtmlEncode(titulo)}</title>
            </head>
            <body style="margin:0;padding:0;background:#f4f7fb;font-family:Arial,Helvetica,sans-serif;display:flex;min-height:100vh;align-items:center;justify-content:center;">
                <div style="background:#ffffff;border-radius:16px;box-shadow:0 6px 18px rgba(0,0,0,0.08);max-width:440px;width:90%;padding:40px 32px;text-align:center;">
                    <div style="font-size:48px;margin-bottom:16px;">{icone}</div>
                    <h1 style="margin:0 0 12px;color:{cor};font-size:24px;">{WebUtility.HtmlEncode(titulo)}</h1>
                    <p style="margin:0;color:#475569;font-size:16px;line-height:1.6;">{WebUtility.HtmlEncode(mensagem)}</p>
                </div>
            </body>
            </html>
            """;
    }

    public static string BuildRedefinirSenhaForm(string token, string? erro = null)
    {
        var tokenSeguro = WebUtility.HtmlEncode(token ?? string.Empty);
        var erroHtml = string.IsNullOrWhiteSpace(erro)
            ? string.Empty
            : $"""<p style="color:#DC2626;font-size:14px;margin:0 0 16px;">{WebUtility.HtmlEncode(erro)}</p>""";

        return $"""
            <!DOCTYPE html>
            <html lang="pt-BR">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Redefinir senha</title>
            </head>
            <body style="margin:0;padding:0;background:#f4f7fb;font-family:Arial,Helvetica,sans-serif;display:flex;min-height:100vh;align-items:center;justify-content:center;">
                <div style="background:#ffffff;border-radius:16px;box-shadow:0 6px 18px rgba(0,0,0,0.08);max-width:400px;width:90%;padding:40px 32px;box-sizing:border-box;">
                    <h1 style="margin:0 0 8px;color:#1E3A5F;font-size:22px;text-align:center;">🔑 Nova senha</h1>
                    <p style="margin:0 0 24px;color:#475569;font-size:14px;text-align:center;">Escolha uma nova senha para sua conta Task Kids.</p>
                    {erroHtml}
                    <form method="post" action="/api/v1/Usuario/RedefinirSenha/Confirmar">
                        <input type="hidden" name="Token" value="{tokenSeguro}" />
                        <label style="display:block;font-size:13px;color:#334155;margin-bottom:6px;">Nova senha</label>
                        <input type="password" name="NovaSenha" minlength="8" required
                            style="width:100%;box-sizing:border-box;padding:12px;border:1px solid #cbd5e1;border-radius:8px;margin-bottom:16px;font-size:15px;" />
                        <label style="display:block;font-size:13px;color:#334155;margin-bottom:6px;">Confirmar nova senha</label>
                        <input type="password" name="ConfirmarSenha" minlength="8" required
                            style="width:100%;box-sizing:border-box;padding:12px;border:1px solid #cbd5e1;border-radius:8px;margin-bottom:24px;font-size:15px;" />
                        <button type="submit"
                            style="width:100%;background:#2D8CF0;color:#ffffff;border:none;padding:14px;border-radius:8px;font-size:16px;font-weight:bold;cursor:pointer;">
                            Redefinir senha
                        </button>
                    </form>
                </div>
            </body>
            </html>
            """;
    }
}
