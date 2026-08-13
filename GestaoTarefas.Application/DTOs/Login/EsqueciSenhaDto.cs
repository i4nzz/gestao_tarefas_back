using System.ComponentModel.DataAnnotations;

namespace GestaoTarefas.Application.DTOs.Login;

public class EsqueciSenhaDto
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;
}
