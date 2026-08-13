using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Login;
using GestaoTarefas.Application.DTOs.Usuario;

namespace GestaoTarefas.Application.Interfaces;
public interface IUsuarioService
{
    Task<RespostaMetodos<IEnumerable<RetornoUsuarioDto>>> ObterTodosAsync();
    Task<RespostaMetodos<RetornoUsuarioDto?>> ObterPorIdAsync(int id);
    Task<RespostaMetodos<RetornoUsuarioDto>> CriarUsuarioAsync(CriarUsuarioDto dto);
    Task<RespostaMetodos<RetornoUsuarioDto>> AtualizarAsync(int id, AtualizarUsuarioDto dto);
    Task<RespostaMetodos<RetornoUsuarioDto>> RemoverAsync(int id);
    Task<RespostaMetodos<RetornoUsuarioDto>> CriarFilhoAsync(CriarFilhoDto dto);
    Task<RespostaMetodos<RetornoLoginDto>> LoginAsync(LoginDto dto);
    Task<RespostaMetodos<RetornoLoginDto>> RefreshTokenAsync(RefreshTokenDto dto);
    Task<RespostaMetodos<object?>> ConfirmarEmailAsync(ConfirmarEmailDto dto);
    Task<RespostaMetodos<object?>> EsqueciSenhaAsync(EsqueciSenhaDto dto);
    Task<RespostaMetodos<object?>> RedefinirSenhaAsync(RedefinirSenhaDto dto);

}
