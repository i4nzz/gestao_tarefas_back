using System.Net;

namespace GestaoTarefas.Application.Common.Responses;

public class RespostaMetodos<T>
{
    public bool Sucesso { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public T? ObjetoRetorno { get; set; }
    public string? Mensagem { get; set; }
    public List<string>? Erros { get; set; }
}