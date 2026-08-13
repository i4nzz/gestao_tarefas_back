using System.Net;
using System.Text.Json;
using GestaoTarefas.Application.Common.Responses;

namespace GestaoTarefas.Middlewares;

/// <summary>
/// Middleware para tratamento global de exceções na API, capturando erros não tratados e retornando respostas JSON padronizadas com mensagens de erro apropriadas e códigos de status HTTP correspondentes.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    /// <summary>
    /// Construtor do middleware de exceção, recebendo o próximo delegate na pipeline e um logger para registrar os erros capturados.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    public ExceptionMiddleware(
        RequestDelegate next
        , ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    /// <summary>
    /// Metodo para invocar o middleware, capturando exceções lançadas durante o processamento da requisição e retornando respostas JSON padronizadas com mensagens de erro e códigos de status HTTP apropriados, além de registrar os erros no log para análise posterior.
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }

        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso não encontrado");
            await EscreverResposta(httpContext, HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Operação inválida");
            await EscreverResposta(httpContext, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Acesso não autorizado");
            await EscreverResposta(httpContext, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? "Sem inner exception";
            _logger.LogError(ex, "Erro inesperado: {Message} | Inner: {Inner}", ex.Message, innerMessage);
            await EscreverResposta(httpContext, HttpStatusCode.InternalServerError, $"{ex.Message} | Inner: {innerMessage}");
        }

    }

    private static async Task EscreverResposta(HttpContext httpContext, HttpStatusCode statusCode, string mensagem)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)statusCode;

        var resposta = new RespostaMetodos<object>
        {
            Sucesso = false,
            StatusCode = statusCode,
            Mensagem = mensagem,
            Erros = new List<string> { mensagem }
        };

        var json = JsonSerializer.Serialize(resposta, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await httpContext.Response.WriteAsync(json);
    }

}
