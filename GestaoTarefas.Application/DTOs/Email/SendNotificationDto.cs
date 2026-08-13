using System.ComponentModel.DataAnnotations;

namespace GestaoTarefas.Application.DTOs.Email;

public class SendNotificationDto
{
    [Required, EmailAddress]
    public string ToEmail { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;
}
