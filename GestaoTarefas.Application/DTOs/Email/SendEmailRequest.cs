namespace GestaoTarefas.Application.DTOs.Email;

public class SendEmailRequest
{
    public List<string> ListaDestinatarios { get; set; } = [];
    public string Assunto { get; set; } = string.Empty;
    public string? CorpoTexto { get; set; } // fallback quando nao tem html
    public string? HtmlBody { get; set; }
    public string? Remetente { get; set; }
    public List<string>? DestinatariosEmCopia { get; set; }
    public List<string>? DestinatariosEmCopiaOculta { get; set; }
    public Dictionary<string, string>? Tags { get; set; }
}
