using System.Net;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Login;
using GestaoTarefas.Application.DTOs.Usuario;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Mapping;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GestaoTarefas.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserService _currentUser;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IEmailService emailService,
        IConfiguration configuration,
        ICurrentUserService currentUser)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _emailService = emailService;
        _configuration = configuration;
        _currentUser = currentUser;
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoUsuarioDto>>> ObterTodosAsync()
    {
        var usuarios = await _usuarioRepository.ObterTodosAsync();

        if (usuarios == null || !usuarios.Any())
        {
            return new RespostaMetodos<IEnumerable<RetornoUsuarioDto>>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Nenhum usuário encontrado"
            };
        }

        var retornoUsuarios = usuarios.ToDtoList();

        return new RespostaMetodos<IEnumerable<RetornoUsuarioDto>>
        {
            Sucesso = true,
            ObjetoRetorno = retornoUsuarios,
            Mensagem = "Usuários obtidos com sucesso"
        };
    }

    public async Task<RespostaMetodos<object?>> EsqueciSenhaAsync(EsqueciSenhaDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(dto.Email);

        if (usuario != null && usuario.Ativo)
        {
            var expiracaoHoras = int.Parse(_configuration["ResetSenha:ExpiracaoHoras"] ?? "1");

            var token = usuario.GerarTokenResetSenha(TimeSpan.FromHours(expiracaoHoras));

            if (string.IsNullOrEmpty(token))
            {
                return new RespostaMetodos<object?>
                {
                    Sucesso = false,
                    ObjetoRetorno = null,
                    StatusCode = HttpStatusCode.BadRequest,
                    Mensagem = "Não foi possível gerar o token de redefinição de senha"
                };
            }

            await _usuarioRepository.AtualizarAsync(usuario);

            var baseUrl = _configuration["LinksEmail:BaseUrl"];
            var path = _configuration["LinksEmail:RedefinicaoSenhaPath"];
            var link = $"{baseUrl}{path}?token={token}";

            var resultadoEnvioEmail = await _emailService.EnviarEmailResetSenhaAsync(usuario.Email, usuario.Nome, link);

            if (!resultadoEnvioEmail.Sucesso)
            {
                return new RespostaMetodos<object?>
                {
                    Sucesso = false,
                    ObjetoRetorno = null,
                    StatusCode = HttpStatusCode.BadRequest,
                    Mensagem = "Não foi possível enviar o e-mail de redefinição de senha"
                };
            }
        }

        return new RespostaMetodos<object?>
        {
            Sucesso = true,
            ObjetoRetorno = null,
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Você receberá instruções para redefinir sua senha."
        };
    }

    public async Task<RespostaMetodos<object?>> RedefinirSenhaAsync(RedefinirSenhaDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorTokenResetSenhaAsync(dto.Token);

        if (usuario == null || !usuario.ValidarTokenResetSenha(dto.Token))
        {
            return new RespostaMetodos<object?>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Token de redefinição inválido ou expirado"
            };
        }

        if (!usuario.EmailConfirmado)
        {
            return new RespostaMetodos<object?>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Confirme seu e-mail antes de redefinir a senha"
            };
        }

        var novaSenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
        usuario.RedefinirSenha(novaSenhaHash);

        await _usuarioRepository.AtualizarAsync(usuario);

        return new RespostaMetodos<object?>
        {
            Sucesso = true,
            ObjetoRetorno = null,
            Mensagem = "Senha redefinida com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoUsuarioDto?>> ObterPorIdAsync(int id)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id);

        if (usuario == null)
        {
            return new RespostaMetodos<RetornoUsuarioDto?>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Usuário não encontrado"
            };
        }

        var retornoUsuario = usuario.ToDto();

        return new RespostaMetodos<RetornoUsuarioDto?>
        {
            Sucesso = true,
            ObjetoRetorno = retornoUsuario,
            Mensagem = "Usuário obtido com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoUsuarioDto>> CriarUsuarioAsync(CriarUsuarioDto dto)
    {
        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(dto.Email);

        if (usuarioExistente != null)
        {
            return new RespostaMetodos<RetornoUsuarioDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Já existe uma conta cadastrada com este e-mail"
            };
        }

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        var pai = new Pai(dto.Nome, dto.Email, senhaHash);

        var expiracaoHoras = int.Parse(_configuration["ConfirmacaoEmail:ExpiracaoHoras"] ?? "24");
        var token = pai.GerarTokenConfirmacaoEmail(TimeSpan.FromHours(expiracaoHoras));

        await _usuarioRepository.AdicionarAsync(pai);

        var baseUrl = _configuration["LinksEmail:BaseUrl"];
        var path = _configuration["LinksEmail:ConfirmacaoEmailPath"];
        var link = $"{baseUrl}{path}?token={token}";

        var resultadoEmail = await _emailService.EnviarEmailConfirmacaoAsync(pai.Email, pai.Nome, link);

        var mensagem = resultadoEmail.Sucesso
            ? "Usuário criado com sucesso. Verifique seu e-mail para confirmar a conta."
            : "Usuário criado com sucesso, mas houve falha ao enviar o e-mail de confirmação.";

        return new RespostaMetodos<RetornoUsuarioDto>
        {
            Sucesso = true,
            ObjetoRetorno = pai.ToDto(),
            Mensagem = mensagem
        };
    }

    public async Task<RespostaMetodos<object?>> ConfirmarEmailAsync(ConfirmarEmailDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorTokenConfirmacaoEmailAsync(dto.Token);

        if (usuario == null)
        {
            return new RespostaMetodos<object?>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Token de confirmação inválido ou expirado"
            };
        }

        var confirmado = usuario.ConfirmarEmail(dto.Token);

        if (!confirmado)
        {
            return new RespostaMetodos<object?>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Token de confirmação inválido ou expirado"
            };
        }

        await _usuarioRepository.AtualizarAsync(usuario);

        return new RespostaMetodos<object?>
        {
            Sucesso = true,
            ObjetoRetorno = null,
            Mensagem = "E-mail confirmado com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoUsuarioDto>> AtualizarAsync(int id, AtualizarUsuarioDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id);
        if (usuario == null)
        {
            return new RespostaMetodos<RetornoUsuarioDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Usuário não encontrado"
            };
        }

        if (id != _currentUser.UsuarioId)
        {
            return new RespostaMetodos<RetornoUsuarioDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você só pode atualizar os seus próprios dados"
            };
        }

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;

        if (!string.IsNullOrWhiteSpace(dto.NovaSenha))
        {
            usuario.RedefinirSenha(BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha));
        }

        await _usuarioRepository.AtualizarAsync(usuario);

        var usuarioRetorno = usuario.ToDto();

        return new RespostaMetodos<RetornoUsuarioDto>
        {
            Sucesso = true,
            ObjetoRetorno = usuarioRetorno,
            Mensagem = "Usuário atualizado com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoUsuarioDto>> RemoverAsync(int id)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id);

        if (usuario == null)
        {
            return new RespostaMetodos<RetornoUsuarioDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Usuário não encontrado"
            };
        }

        if (id != _currentUser.UsuarioId)
        {
            return new RespostaMetodos<RetornoUsuarioDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você só pode remover a sua própria conta"
            };
        }

        if (await _usuarioRepository.PossuiVinculoFamiliarAsync(id))
        {
            return new RespostaMetodos<RetornoUsuarioDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Conflict,
                Mensagem = usuario.Perfil == PerfilUsuarioEnum.Pai
                    ? "Não é possível remover esta conta: existem filhos vinculados a ela. Desvincule-os antes de remover."
                    : "Não é possível remover esta conta: ela está vinculada a um responsável. Peça para o seu responsável desvincular a conta antes de remover."
            };
        }

        await _usuarioRepository.RemoverAsync(id);

        return new RespostaMetodos<RetornoUsuarioDto>
        {
            Sucesso = true,
            ObjetoRetorno = null,
            Mensagem = "Usuário removido com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoUsuarioDto>> CriarFilhoAsync(CriarFilhoDto dto)
    {
        var paiId = _currentUser.UsuarioId;
        var pai = await _usuarioRepository.ObterPorIdAsync(paiId);

        if (pai == null)
        {
            return new RespostaMetodos<RetornoUsuarioDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Pai não encontrado"
            };
        }

        if (pai.Perfil != PerfilUsuarioEnum.Pai)
        {
            return new RespostaMetodos<RetornoUsuarioDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "O usuário informado não é um Pai"
            };
        }

        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(dto.Email);

        if (usuarioExistente != null)
        {
            return new RespostaMetodos<RetornoUsuarioDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Já existe uma conta cadastrada com este e-mail"
            };
        }

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        var filho = new Filho(dto.Nome, dto.Email, senhaHash, dto.DataNascimento);
        var vinculo = new PaisFilhos(paiId, 0); // o id é passado na repository

        await _usuarioRepository.AdicionarFilhoAsync(filho, vinculo);

        return new RespostaMetodos<RetornoUsuarioDto>
        {
            Sucesso = true,
            ObjetoRetorno = filho.ToDto(),
            Mensagem = "Filho cadastrado com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoLoginDto>> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(dto.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
        {
            return new RespostaMetodos<RetornoLoginDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Email ou senha inválidos"
            };
        }

        if (!usuario.Ativo)
        {
            return new RespostaMetodos<RetornoLoginDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Conta desativada. Entre em contato com o suporte."
            };
        }

        if (!usuario.EmailConfirmado)
        {
            return new RespostaMetodos<RetornoLoginDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Confirme seu e-mail antes de fazer login. Verifique sua caixa de entrada ou a caixa de Spam."
            };
        }

        var (accessToken, expiracao) = _tokenService.GerarAccessToken(usuario);
        var refreshTokenValor = _tokenService.GerarRefreshToken();
        var validadeRefreshToken = _tokenService.ObterValidadeRefreshToken();

        var refreshToken = new RefreshToken(usuario.Id, refreshTokenValor, validadeRefreshToken);
        await _refreshTokenRepository.AdicionarAsync(refreshToken);

        return new RespostaMetodos<RetornoLoginDto>
        {
            Sucesso = true,
            ObjetoRetorno = new RetornoLoginDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValor,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                Expiracao = expiracao
            },
            Mensagem = "Login realizado com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoLoginDto>> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var refreshTokenAtual = await _refreshTokenRepository.ObterPorTokenAsync(dto.RefreshToken);

        if (refreshTokenAtual == null || !refreshTokenAtual.EstaAtivo)
        {
            return new RespostaMetodos<RetornoLoginDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Refresh token inválido ou expirado"
            };
        }

        var usuario = await _usuarioRepository.ObterPorIdAsync(refreshTokenAtual.UsuarioId);

        if (usuario == null)
        {
            return new RespostaMetodos<RetornoLoginDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Usuário não encontrado"
            };
        }

        var (accessToken, expiracao) = _tokenService.GerarAccessToken(usuario);
        var novoRefreshTokenValor = _tokenService.GerarRefreshToken();
        var validadeRefreshToken = _tokenService.ObterValidadeRefreshToken();
        var novoRefreshToken = new RefreshToken(usuario.Id, novoRefreshTokenValor, validadeRefreshToken);

        refreshTokenAtual.Revogar(novoRefreshTokenValor);

        await _refreshTokenRepository.AtualizarAsync(refreshTokenAtual);
        await _refreshTokenRepository.AdicionarAsync(novoRefreshToken);

        return new RespostaMetodos<RetornoLoginDto>
        {
            Sucesso = true,
            ObjetoRetorno = new RetornoLoginDto
            {
                AccessToken = accessToken,
                RefreshToken = novoRefreshTokenValor,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                Expiracao = expiracao
            },
            Mensagem = "Token renovado com sucesso"
        };
    }
}

