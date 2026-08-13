using System.ComponentModel.DataAnnotations;

namespace GestaoTarefas.Application.DTOs.Email;

public class SendSimpleEmailDto
{
    [Required(ErrorMessage = "O destinatário é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    public string To { get; set; } = string.Empty;

    [Required(ErrorMessage = "O assunto é obrigatório.")]
    [MaxLength(200, ErrorMessage = "Assunto não pode ultrapassar 200 caracteres.")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "O corpo do e-mail é obrigatório.")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Quando true, o campo Body é tratado como HTML.
    /// Quando false (padrão), é tratado como texto puro.
    /// </summary>
    public bool IsHtml { get; set; } = false;
}
