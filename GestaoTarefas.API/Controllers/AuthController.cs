using System.Net;
using GestaoTarefas.Application.DTOs.Login;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Controllers.v1;
/// <summary>
/// Controller para autenticação de usuários, responsável por receber as credenciais de login, validar essas credenciais e retornar um token JWT para os usuários autenticados, permitindo o acesso aos recursos protegidos da API.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    /// <summary>
    /// Construtor para receber a dependência do serviço de usuário via injeção de dependência, permitindo que a controller utilize os métodos do serviço para autenticar os usuários e gerar tokens JWT conforme necessário.
    /// </summary>
    /// <param name="usuarioService"></param>
    public AuthController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }
    /// <summary>
    /// Retorna o token JWT.
    /// </summary>
    /// <param name="dto">Dados de login</param>
    /// <returns>Token JWT</returns>
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _usuarioService.LoginAsync(dto);

        if (!resultado.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.Unauthorized, resultado);
        }

        return StatusCode((int)HttpStatusCode.OK, resultado.ObjetoRetorno);
    }
    /// <summary>
    /// Confirma o e-mail do usuário a partir do token recebido por e-mail.
    /// </summary>
    [HttpPost("ConfirmarEmail")]
    public async Task<IActionResult> ConfirmarEmail([FromBody] ConfirmarEmailDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _usuarioService.ConfirmarEmailAsync(dto);

        if (!resultado.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, resultado);
        }

        return StatusCode((int)HttpStatusCode.OK, resultado);
    }


    /// <summary>
    /// Gera um novo par de tokens (access + refresh) a partir de um refresh token válido, revogando o antigo.
    /// </summary>
    /// <param name="dto">Refresh token atual</param>
    /// <returns>Novo par de tokens</returns>
    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _usuarioService.RefreshTokenAsync(dto);

        if (!resultado.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.Unauthorized, resultado);
        }

        return StatusCode((int)HttpStatusCode.OK, resultado.ObjetoRetorno);
    }
}