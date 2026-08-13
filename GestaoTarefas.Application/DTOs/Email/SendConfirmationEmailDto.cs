using System.ComponentModel.DataAnnotations;

namespace GestaoTarefas.Application.DTOs.Email;

public class SendConfirmationEmailDto
{
    [Required(ErrorMessage = "O e-mail do destinatário é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    public string ToEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O link é obrigatório.")]
    [Url(ErrorMessage = "O link deve ser uma URL válida.")]
    public string Link { get; set; } = string.Empty;
}
