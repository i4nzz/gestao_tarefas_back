using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;
using Moq;
using Xunit;

namespace Usuarios.API.Tests;

public class AutorizacaoFamiliarServiceTests
{
    [Fact]
    public async Task PodeAcessarFilhoAsync_QuandoFilhoAcessaProprioId_RetornaTrue()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.Perfil).Returns(PerfilUsuarioEnum.Filho);
        currentUser.Setup(c => c.UsuarioId).Returns(10);

        var usuarioRepository = new Mock<IUsuarioRepository>();
        var servico = new AutorizacaoFamiliarService(currentUser.Object, usuarioRepository.Object);

        var resultado = await servico.PodeAcessarFilhoAsync(10);

        Assert.True(resultado);
        usuarioRepository.Verify(r => r.ExisteVinculoAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task PodeAcessarFilhoAsync_QuandoFilhoAcessaOutroFilho_RetornaFalse()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.Perfil).Returns(PerfilUsuarioEnum.Filho);
        currentUser.Setup(c => c.UsuarioId).Returns(10);

        var usuarioRepository = new Mock<IUsuarioRepository>();
        var servico = new AutorizacaoFamiliarService(currentUser.Object, usuarioRepository.Object);

        var resultado = await servico.PodeAcessarFilhoAsync(99);

        Assert.False(resultado);
    }

    [Fact]
    public async Task PodeAcessarFilhoAsync_QuandoPaiTemVinculoComFilho_RetornaTrue()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.Perfil).Returns(PerfilUsuarioEnum.Pai);
        currentUser.Setup(c => c.UsuarioId).Returns(1);

        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ExisteVinculoAsync(1, 10)).ReturnsAsync(true);

        var servico = new AutorizacaoFamiliarService(currentUser.Object, usuarioRepository.Object);

        var resultado = await servico.PodeAcessarFilhoAsync(10);

        Assert.True(resultado);
    }

    [Fact]
    public async Task PodeAcessarFilhoAsync_QuandoPaiNaoTemVinculoComFilho_RetornaFalse()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.Perfil).Returns(PerfilUsuarioEnum.Pai);
        currentUser.Setup(c => c.UsuarioId).Returns(1);

        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ExisteVinculoAsync(1, 55)).ReturnsAsync(false);

        var servico = new AutorizacaoFamiliarService(currentUser.Object, usuarioRepository.Object);

        var resultado = await servico.PodeAcessarFilhoAsync(55);

        Assert.False(resultado);
    }
}
