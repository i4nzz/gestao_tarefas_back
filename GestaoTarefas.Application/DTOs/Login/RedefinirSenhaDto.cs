using System.ComponentModel.DataAnnotations;

namespace GestaoTarefas.Application.DTOs.Login;

public class RedefinirSenhaDto
{
    [Required(ErrorMessage = "O token é obrigatório.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nova senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
    public string NovaSenha { get; set; } = string.Empty;
}
