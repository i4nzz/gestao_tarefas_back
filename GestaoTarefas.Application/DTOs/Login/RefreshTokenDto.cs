using System.ComponentModel.DataAnnotations;

namespace GestaoTarefas.Application.DTOs.Login;

public class RefreshTokenDto
{
    [Required(ErrorMessage = "O refresh token é obrigatório.")]
    public string RefreshToken { get; set; } = string.Empty;
}
