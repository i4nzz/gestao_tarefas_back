using System.ComponentModel.DataAnnotations;

namespace GestaoTarefas.Application.DTOs.Login;

public class ConfirmarEmailDto
{
    [Required(ErrorMessage = "O token é obrigatório.")]
    public string Token { get; set; } = string.Empty;
}
