using System.Net;
using GestaoTarefas.Application.DTOs.Recompensa;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using Moq;
using Xunit;

namespace Usuarios.API.Tests;

public class RecompensaServiceTests
{
    private static RecompensaService CriarServico(
        Mock<IRecompensaRepository> recompensaRepository,
        Mock<IAutorizacaoFamiliarService> autorizacao,
        Mock<IUsuarioRepository>? usuarioRepository = null,
        Mock<IPontuacaoRepository>? pontuacaoRepository = null,
        Mock<IResgatePontuacaoRepository>? resgatePontuacaoRepository = null)
    {
        return new RecompensaService(
            recompensaRepository.Object,
            (pontuacaoRepository ?? new Mock<IPontuacaoRepository>()).Object,
            (usuarioRepository ?? new Mock<IUsuarioRepository>()).Object,
            (resgatePontuacaoRepository ?? new Mock<IResgatePontuacaoRepository>()).Object,
            autorizacao.Object);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoTentaMoverParaFilhoSemVinculo_RetornaForbiddenENaoSalva()
    {
        var recompensa = new Recompensa { Id = 1, FilhoId = 10, Descricao = "Bicicleta", PontosNecessarios = 100 };

        var recompensaRepository = new Mock<IRecompensaRepository>();
        recompensaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(recompensa);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(99)).ReturnsAsync(false);

        var servico = CriarServico(recompensaRepository, autorizacao);

        var dto = new CriarRecompensaDto { FilhoId = 99, Descricao = "Bicicleta", PontosNecessarios = 1 };
        var resultado = await servico.AtualizarAsync(1, dto);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Forbidden, resultado.StatusCode);
        Assert.Equal(10, recompensa.FilhoId);
        recompensaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Recompensa>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoFilhoIdNaoMuda_AtualizaSemChecarVinculoDoNovoFilho()
    {
        var recompensa = new Recompensa { Id = 1, FilhoId = 10, Descricao = "Bicicleta", PontosNecessarios = 100 };

        var recompensaRepository = new Mock<IRecompensaRepository>();
        recompensaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(recompensa);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var servico = CriarServico(recompensaRepository, autorizacao);

        var dto = new CriarRecompensaDto { FilhoId = 10, Descricao = "Jogo novo", PontosNecessarios = 50 };
        var resultado = await servico.AtualizarAsync(1, dto);

        Assert.True(resultado.Sucesso);
        Assert.Equal("Jogo novo", recompensa.Descricao);
        Assert.Equal(50, recompensa.PontosNecessarios);
        recompensaRepository.Verify(r => r.AtualizarAsync(recompensa), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoMoveParaFilhoVinculado_AtualizaComSucesso()
    {
        var recompensa = new Recompensa { Id = 1, FilhoId = 10, Descricao = "Bicicleta", PontosNecessarios = 100 };

        var recompensaRepository = new Mock<IRecompensaRepository>();
        recompensaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(recompensa);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(20)).ReturnsAsync(true);

        var servico = CriarServico(recompensaRepository, autorizacao);

        var dto = new CriarRecompensaDto { FilhoId = 20, Descricao = "Bicicleta", PontosNecessarios = 100 };
        var resultado = await servico.AtualizarAsync(1, dto);

        Assert.True(resultado.Sucesso);
        Assert.Equal(20, recompensa.FilhoId);
        recompensaRepository.Verify(r => r.AtualizarAsync(recompensa), Times.Once);
    }

    [Fact]
    public async Task ResgatarAsync_QuandoSaldoSuficiente_DebitaApenasEmResgatePontuacao()
    {
        var recompensa = new Recompensa { Id = 1, FilhoId = 10, Descricao = "Bicicleta", PontosNecessarios = 100, Ativa = true };

        var recompensaRepository = new Mock<IRecompensaRepository>();
        recompensaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(recompensa);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var pontuacaoRepository = new Mock<IPontuacaoRepository>();
        pontuacaoRepository.Setup(r => r.ObterTotalPontosAsync(10)).ReturnsAsync(150);

        var resgatePontuacaoRepository = new Mock<IResgatePontuacaoRepository>();
        resgatePontuacaoRepository.Setup(r => r.ObterTotalResgatesAsync(10)).ReturnsAsync(0);

        var servico = CriarServico(recompensaRepository, autorizacao, pontuacaoRepository: pontuacaoRepository, resgatePontuacaoRepository: resgatePontuacaoRepository);

        var resultado = await servico.ResgatarAsync(10, 1);

        Assert.True(resultado.Sucesso);
        pontuacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Pontuacao>()), Times.Never);
        resgatePontuacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<ResgatePontuacao>()), Times.Once);
    }

    [Fact]
    public async Task ResgatarAsync_QuandoSaldoLiquidoInsuficiente_NaoResgata()
    {
        var recompensa = new Recompensa { Id = 1, FilhoId = 10, Descricao = "Bicicleta", PontosNecessarios = 100, Ativa = true };

        var recompensaRepository = new Mock<IRecompensaRepository>();
        recompensaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(recompensa);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var pontuacaoRepository = new Mock<IPontuacaoRepository>();
        pontuacaoRepository.Setup(r => r.ObterTotalPontosAsync(10)).ReturnsAsync(150);

        var resgatePontuacaoRepository = new Mock<IResgatePontuacaoRepository>();
        resgatePontuacaoRepository.Setup(r => r.ObterTotalResgatesAsync(10)).ReturnsAsync(80);

        var servico = CriarServico(recompensaRepository, autorizacao, pontuacaoRepository: pontuacaoRepository, resgatePontuacaoRepository: resgatePontuacaoRepository);

        var resultado = await servico.ResgatarAsync(10, 1);

        Assert.False(resultado.Sucesso);
        resgatePontuacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<ResgatePontuacao>()), Times.Never);
    }
}
