using System.Net;
using GestaoTarefas.Application.DTOs.Usuario;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace GestaoTarefas.Tests;

public class UsuarioServiceTests
{
    private static UsuarioService CriarServico(
        Mock<IUsuarioRepository> usuarioRepository,
        Mock<ICurrentUserService> currentUser,
        Mock<ITokenService>? tokenService = null,
        Mock<IRefreshTokenRepository>? refreshTokenRepository = null,
        Mock<IEmailService>? emailService = null,
        Mock<IConfiguration>? configuration = null)
    {
        return new UsuarioService(
            usuarioRepository.Object,
            (tokenService ?? new Mock<ITokenService>()).Object,
            (refreshTokenRepository ?? new Mock<IRefreshTokenRepository>()).Object,
            (emailService ?? new Mock<IEmailService>()).Object,
            (configuration ?? new Mock<IConfiguration>()).Object,
            currentUser.Object);
    }

    [Fact]
    public async Task CriarFilhoAsync_UsaOPaiAutenticado_IgnorandoQualquerIdExterno()
    {
        var pai = new Pai("Pai A", "pai@teste.com", "hash");
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(pai);

        PaisFilhos? vinculoCapturado = null;
        usuarioRepository
            .Setup(r => r.AdicionarFilhoAsync(It.IsAny<Filho>(), It.IsAny<PaisFilhos>()))
            .Callback<Filho, PaisFilhos>((_, vinculo) => vinculoCapturado = vinculo)
            .Returns(Task.CompletedTask);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(1);

        var servico = CriarServico(usuarioRepository, currentUser);

        var dto = new CriarFilhoDto
        {
            Nome = "Filho A",
            Email = "filho@teste.com",
            Senha = "123456",
            DataNascimento = new DateTime(2015, 1, 1)
        };

        var resultado = await servico.CriarFilhoAsync(dto);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(vinculoCapturado);
        Assert.Equal(1, vinculoCapturado!.PaiId);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoIdNaoEDoUsuarioAutenticado_RetornaForbiddenENaoSalva()
    {
        var usuario = new Usuario("Filho A", "a@teste.com", "hash", PerfilUsuarioEnum.Filho);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(usuario);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(99);

        var servico = CriarServico(usuarioRepository, currentUser);

        var dto = new AtualizarUsuarioDto { Nome = "Outro nome", Email = "outro@teste.com" };
        var resultado = await servico.AtualizarAsync(10, dto);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Forbidden, resultado.StatusCode);
        usuarioRepository.Verify(r => r.AtualizarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoNovaSenhaInformada_AtualizaHashDaSenha()
    {
        var usuario = new Usuario("Filho A", "a@teste.com", "hashAntigo", PerfilUsuarioEnum.Filho);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(usuario);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(10);

        var servico = CriarServico(usuarioRepository, currentUser);

        var dto = new AtualizarUsuarioDto { Nome = "Filho A", Email = "a@teste.com", NovaSenha = "novaSenha123" };
        var resultado = await servico.AtualizarAsync(10, dto);

        Assert.True(resultado.Sucesso);
        Assert.NotEqual("hashAntigo", usuario.SenhaHash);
        usuarioRepository.Verify(r => r.AtualizarAsync(usuario), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_QuandoIdNaoEDoUsuarioAutenticado_RetornaForbiddenENaoRemove()
    {
        var usuario = new Usuario("Filho A", "a@teste.com", "hash", PerfilUsuarioEnum.Filho);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(usuario);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(99);

        var servico = CriarServico(usuarioRepository, currentUser);

        var resultado = await servico.RemoverAsync(10);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Forbidden, resultado.StatusCode);
        usuarioRepository.Verify(r => r.RemoverAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_QuandoPossuiVinculoFamiliar_RetornaConflitoENaoRemove()
    {
        var usuario = new Usuario("Pai A", "pai@teste.com", "hash", PerfilUsuarioEnum.Pai);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
        usuarioRepository.Setup(r => r.PossuiVinculoFamiliarAsync(1)).ReturnsAsync(true);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(1);

        var servico = CriarServico(usuarioRepository, currentUser);

        var resultado = await servico.RemoverAsync(1);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Conflict, resultado.StatusCode);
        usuarioRepository.Verify(r => r.RemoverAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_QuandoSemVinculoFamiliar_Remove()
    {
        var usuario = new Usuario("Filho A", "filho@teste.com", "hash", PerfilUsuarioEnum.Filho);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
        usuarioRepository.Setup(r => r.PossuiVinculoFamiliarAsync(1)).ReturnsAsync(false);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(1);

        var servico = CriarServico(usuarioRepository, currentUser);

        var resultado = await servico.RemoverAsync(1);

        Assert.True(resultado.Sucesso);
        usuarioRepository.Verify(r => r.RemoverAsync(1), Times.Once);
    }
}
