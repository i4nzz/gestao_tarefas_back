namespace GestaoTarefas.Application.Configuration;

public class BrevoOptions
{
    public const string SectionName = "Brevo";
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultFromEmail { get; set; } = string.Empty;
    public string DefaultFromName { get; set; } = "Minha Aplicação";
}
