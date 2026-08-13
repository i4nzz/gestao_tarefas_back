using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestaoTarefas.Application.DTOs.Recompensa;
public class CriarComprovacaoDto
{
    [Required]
    public int TarefaId { get; set; }
    [Required]
    public IFormFile Foto { get; set; } = null!;
}
