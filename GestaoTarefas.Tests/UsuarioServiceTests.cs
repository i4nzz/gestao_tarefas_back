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
    public async Task CriarFilhoAsync_MarcaEmailComoConfirmado_ParaPermitirLoginImediato()
    {
        var pai = new Pai("Pai A", "pai@teste.com", "hash");
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(pai);

        Filho? filhoCapturado = null;
        usuarioRepository
            .Setup(r => r.AdicionarFilhoAsync(It.IsAny<Filho>(), It.IsAny<PaisFilhos>()))
            .Callback<Filho, PaisFilhos>((filho, _) => filhoCapturado = filho)
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
        Assert.NotNull(filhoCapturado);
        Assert.True(filhoCapturado!.EmailConfirmado);
    }

    [Fact]
    public async Task ObterMeusFilhosAsync_RetornaApenasFilhosDoPaiAutenticado()
    {
        var filho = new Filho("Filho A", "filho@teste.com", "hash", new DateTime(2015, 1, 1));
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterFilhosPorPaiIdAsync(1)).ReturnsAsync(new List<Usuario> { filho });

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(1);

        var servico = CriarServico(usuarioRepository, currentUser);

        var resultado = await servico.ObterMeusFilhosAsync();

        Assert.True(resultado.Sucesso);
        Assert.Single(resultado.ObjetoRetorno!);
        usuarioRepository.Verify(r => r.ObterFilhosPorPaiIdAsync(1), Times.Once);
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
    public async Task AtualizarAsync_QuandoPaiTemVinculoComFilho_AtualizaOsDadosDoFilho()
    {
        var filho = new Usuario("Filho A", "a@teste.com", "hash", PerfilUsuarioEnum.Filho);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(filho);
        usuarioRepository.Setup(r => r.ExisteVinculoAsync(1, 10)).ReturnsAsync(true);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(1);
        currentUser.Setup(c => c.Perfil).Returns(PerfilUsuarioEnum.Pai);

        var servico = CriarServico(usuarioRepository, currentUser);

        var dto = new AtualizarUsuarioDto { Nome = "Filho Renomeado", Email = "novo@teste.com" };
        var resultado = await servico.AtualizarAsync(10, dto);

        Assert.True(resultado.Sucesso);
        Assert.Equal("Filho Renomeado", filho.Nome);
        Assert.Equal("novo@teste.com", filho.Email);
        usuarioRepository.Verify(r => r.AtualizarAsync(filho), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoPaiNaoTemVinculoComFilho_RetornaForbiddenENaoSalva()
    {
        var filho = new Usuario("Filho A", "a@teste.com", "hash", PerfilUsuarioEnum.Filho);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(filho);
        usuarioRepository.Setup(r => r.ExisteVinculoAsync(1, 10)).ReturnsAsync(false);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(1);
        currentUser.Setup(c => c.Perfil).Returns(PerfilUsuarioEnum.Pai);

        var servico = CriarServico(usuarioRepository, currentUser);

        var dto = new AtualizarUsuarioDto { Nome = "Outro nome", Email = "outro@teste.com" };
        var resultado = await servico.AtualizarAsync(10, dto);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Forbidden, resultado.StatusCode);
        usuarioRepository.Verify(r => r.AtualizarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task AlterarStatusAsync_QuandoPaiTemVinculoComFilho_InativaOUsuario()
    {
        var filho = new Usuario("Filho A", "a@teste.com", "hash", PerfilUsuarioEnum.Filho);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(filho);
        usuarioRepository.Setup(r => r.ExisteVinculoAsync(1, 10)).ReturnsAsync(true);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(1);
        currentUser.Setup(c => c.Perfil).Returns(PerfilUsuarioEnum.Pai);

        var servico = CriarServico(usuarioRepository, currentUser);

        var resultado = await servico.AlterarStatusAsync(10, new AlterarStatusUsuarioDto { Ativo = false });

        Assert.True(resultado.Sucesso);
        Assert.False(filho.Ativo);
        usuarioRepository.Verify(r => r.AtualizarAsync(filho), Times.Once);
    }

    [Fact]
    public async Task AlterarStatusAsync_QuandoPaiNaoTemVinculoComFilho_RetornaForbiddenENaoAltera()
    {
        var filho = new Usuario("Filho A", "a@teste.com", "hash", PerfilUsuarioEnum.Filho);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(filho);
        usuarioRepository.Setup(r => r.ExisteVinculoAsync(1, 10)).ReturnsAsync(false);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(1);
        currentUser.Setup(c => c.Perfil).Returns(PerfilUsuarioEnum.Pai);

        var servico = CriarServico(usuarioRepository, currentUser);

        var resultado = await servico.AlterarStatusAsync(10, new AlterarStatusUsuarioDto { Ativo = false });

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Forbidden, resultado.StatusCode);
        Assert.True(filho.Ativo);
        usuarioRepository.Verify(r => r.AtualizarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task AlterarStatusAsync_QuandoUsuarioAutenticadoNaoEPai_RetornaForbidden()
    {
        var outroFilho = new Usuario("Filho B", "b@teste.com", "hash", PerfilUsuarioEnum.Filho);
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(outroFilho);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UsuarioId).Returns(5);
        currentUser.Setup(c => c.Perfil).Returns(PerfilUsuarioEnum.Filho);

        var servico = CriarServico(usuarioRepository, currentUser);

        var resultado = await servico.AlterarStatusAsync(10, new AlterarStatusUsuarioDto { Ativo = false });

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Forbidden, resultado.StatusCode);
        usuarioRepository.Verify(r => r.AtualizarAsync(It.IsAny<Usuario>()), Times.Never);
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
