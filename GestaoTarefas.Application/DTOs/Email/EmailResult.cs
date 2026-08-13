namespace GestaoTarefas.Application.DTOs.Email;

public class EmailResult
{
    public bool Sucesso { get; init; }
    // ID retornado pelo Resend para rastreamento.
    public string? EmailId { get; init; }
    public string? MensagemErro { get; init; }
    public static EmailResult Ok(string emailId)
    {
        return new() { Sucesso = true, EmailId = emailId };
    }
    public static EmailResult Fail(string mensagemErro)
    {
        return new() { Sucesso = false, MensagemErro = mensagemErro };
    }
}
