using System.Net;
using GestaoTarefas.Application.DTOs.Login;
using GestaoTarefas.Application.DTOs.Usuario;
using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Controllers.v1;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuarioService"></param>
    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }
    /// <summary>
    /// Obter todos os usuários cadastrados.
    /// </summary>
    /// <returns>Lista de usuários</returns>
    [HttpGet]
    [Route("ObterTodos")]
    public async Task<IActionResult> ObterTodos()
    {
        var usuarios = await _usuarioService.ObterTodosAsync();

        if (!usuarios.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.NotFound, usuarios);
        }

        return StatusCode((int)HttpStatusCode.OK, usuarios.ObjetoRetorno);
    }
    /// <summary>
    /// Obter um usuário específico pelo seu ID.
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Usuário correspondente ao ID fornecido</returns>
    [HttpGet]
    [Route("ObterPorId/{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(id);

        if (!usuario.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.NotFound, usuario);
        }

        return StatusCode((int)HttpStatusCode.OK, usuario.ObjetoRetorno);
    }
    /// <summary>
    /// Criar um novo usuario. Usuario padrão é o Pai.
    /// </summary>
    /// <param name="dto">Dados para criar o usuário</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost]
    [Route("AdicionarUsuarioPai")]
    [AllowAnonymous]
    public async Task<IActionResult> Criar([FromBody] CriarUsuarioDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usuario = await _usuarioService.CriarUsuarioAsync(dto);

        if (!usuario.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, usuario);
        }

        return StatusCode((int)HttpStatusCode.Created, usuario.ObjetoRetorno);
    }
    /// <summary>
    /// Atualizar as informações de um usuário existente, como nome, email, senha e perfil.
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="dto">Dados para atualizar o usuário</param>
    /// <returns>Resultado da operação</returns>
    [HttpPut]
    [Route("AtualizarUsuario/{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuarioDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usuario = await _usuarioService.AtualizarAsync(id, dto);

        if (!usuario.Sucesso)
        {
            if (usuario.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, usuario);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, usuario);
        }

        return StatusCode((int)HttpStatusCode.OK, usuario.ObjetoRetorno);
    }
    /// <summary>
    /// Remover a própria conta do sistema. Se houver vínculo familiar (Pai com filhos, ou Filho vinculado a um
    /// responsável), a remoção é bloqueada com 409 Conflict até que o vínculo seja desfeito.
    /// </summary>
    /// <param name="id">ID do usuário (deve ser o mesmo do usuário autenticado)</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete]
    [Route("RemoverUsuario/{id}")]
    public async Task<IActionResult> Remover(int id)
    {
        var usuario = await _usuarioService.RemoverAsync(id);

        if (!usuario.Sucesso)
        {
            if (usuario.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, usuario);
            }

            if (usuario.StatusCode == HttpStatusCode.Conflict)
            {
                return StatusCode((int)HttpStatusCode.Conflict, usuario);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, usuario);
        }

        return StatusCode((int)HttpStatusCode.OK, usuario.ObjetoRetorno);
    }
    /// <summary>
    /// Criar um usuario tipo Filho.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("AdicionarFilho")]
    [Authorize(Roles = "Pai")]
    public async Task<IActionResult> CriarFilho([FromBody] CriarFilhoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _usuarioService.CriarFilhoAsync(dto);

        if (!resultado.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, resultado);
        }

        return StatusCode((int)HttpStatusCode.Created, resultado.ObjetoRetorno);
    }

    /// <summary>
    /// Solicita redefinição de senha. Sempre retorna a mesma resposta, exista ou não o e-mail informado.
    /// </summary>
    [HttpPost("EsqueciSenha")]
    [AllowAnonymous]
    public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _usuarioService.EsqueciSenhaAsync(dto);

        return StatusCode((int)HttpStatusCode.OK, resultado);
    }

    /// <summary>
    /// Redefine a senha a partir de um token válido recebido por e-mail.
    /// </summary>
    [HttpPost("RedefinirSenha")]
    [AllowAnonymous]
    public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _usuarioService.RedefinirSenhaAsync(dto);

        if (!resultado.Sucesso)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, resultado);
        }

        return StatusCode((int)HttpStatusCode.OK, resultado);
    }
}