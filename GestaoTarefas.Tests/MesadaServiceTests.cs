using System.Net;
using GestaoTarefas.Application.DTOs.Mesada;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using Moq;
using Xunit;

namespace GestaoTarefas.Tests;

public class MesadaServiceTests
{
    private static MesadaService CriarServico(
        Mock<IMesadaRepository> mesadaRepository,
        Mock<IUsuarioRepository> usuarioRepository,
        Mock<IRegistroFinanceiroRepository> registroRepository,
        Mock<IAutorizacaoFamiliarService> autorizacao)
    {
        return new MesadaService(
            mesadaRepository.Object,
            usuarioRepository.Object,
            registroRepository.Object,
            autorizacao.Object);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoValorMenorQueTotalGasto_NaoAtualiza()
    {
        var mesada = new Mesada { MesadaId = 1, FilhoId = 10, Valor = 100, Mes = 8, Ano = 2026 };

        var mesadaRepository = new Mock<IMesadaRepository>();
        mesadaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(mesada);

        var registroRepository = new Mock<IRegistroFinanceiroRepository>();
        registroRepository.Setup(r => r.ObterTotalGastoPorMesadaAsync(1)).ReturnsAsync(80);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var servico = CriarServico(mesadaRepository, new Mock<IUsuarioRepository>(), registroRepository, autorizacao);

        var resultado = await servico.AtualizarAsync(1, new AtualizarMesadaDto { Valor = 50 });

        Assert.False(resultado.Sucesso);
        mesadaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Mesada>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoValorValido_Atualiza()
    {
        var mesada = new Mesada { MesadaId = 1, FilhoId = 10, Valor = 100, Mes = 8, Ano = 2026 };

        var mesadaRepository = new Mock<IMesadaRepository>();
        mesadaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(mesada);

        var registroRepository = new Mock<IRegistroFinanceiroRepository>();
        registroRepository.Setup(r => r.ObterTotalGastoPorMesadaAsync(1)).ReturnsAsync(30);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var servico = CriarServico(mesadaRepository, new Mock<IUsuarioRepository>(), registroRepository, autorizacao);

        var resultado = await servico.AtualizarAsync(1, new AtualizarMesadaDto { Valor = 150 });

        Assert.True(resultado.Sucesso);
        Assert.Equal(150, mesada.Valor);
        mesadaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Mesada>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoSemVinculo_RetornaForbidden()
    {
        var mesada = new Mesada { MesadaId = 1, FilhoId = 10, Valor = 100, Mes = 8, Ano = 2026 };

        var mesadaRepository = new Mock<IMesadaRepository>();
        mesadaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(mesada);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(false);

        var servico = CriarServico(mesadaRepository, new Mock<IUsuarioRepository>(), new Mock<IRegistroFinanceiroRepository>(), autorizacao);

        var resultado = await servico.AtualizarAsync(1, new AtualizarMesadaDto { Valor = 150 });

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Forbidden, resultado.StatusCode);
    }

    [Fact]
    public async Task RemoverAsync_PrimeiraChamada_Desativa()
    {
        var mesada = new Mesada { MesadaId = 1, FilhoId = 10, Valor = 100, Mes = 8, Ano = 2026, Ativa = true };

        var mesadaRepository = new Mock<IMesadaRepository>();
        mesadaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(mesada);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var servico = CriarServico(mesadaRepository, new Mock<IUsuarioRepository>(), new Mock<IRegistroFinanceiroRepository>(), autorizacao);

        var resultado = await servico.RemoverAsync(1);

        Assert.True(resultado.Sucesso);
        Assert.False(mesada.Ativa);
        mesadaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Mesada>()), Times.Once);
        mesadaRepository.Verify(r => r.RemoverAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_SegundaChamadaSemGastos_RemoveDeVerdade()
    {
        var mesada = new Mesada { MesadaId = 1, FilhoId = 10, Valor = 100, Mes = 8, Ano = 2026, Ativa = false };

        var mesadaRepository = new Mock<IMesadaRepository>();
        mesadaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(mesada);

        var registroRepository = new Mock<IRegistroFinanceiroRepository>();
        registroRepository.Setup(r => r.ObterTotalGastoPorMesadaAsync(1)).ReturnsAsync(0);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var servico = CriarServico(mesadaRepository, new Mock<IUsuarioRepository>(), registroRepository, autorizacao);

        var resultado = await servico.RemoverAsync(1);

        Assert.True(resultado.Sucesso);
        mesadaRepository.Verify(r => r.RemoverAsync(1), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_SegundaChamadaComGastos_RetornaConflict()
    {
        var mesada = new Mesada { MesadaId = 1, FilhoId = 10, Valor = 100, Mes = 8, Ano = 2026, Ativa = false };

        var mesadaRepository = new Mock<IMesadaRepository>();
        mesadaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(mesada);

        var registroRepository = new Mock<IRegistroFinanceiroRepository>();
        registroRepository.Setup(r => r.ObterTotalGastoPorMesadaAsync(1)).ReturnsAsync(30);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var servico = CriarServico(mesadaRepository, new Mock<IUsuarioRepository>(), registroRepository, autorizacao);

        var resultado = await servico.RemoverAsync(1);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Conflict, resultado.StatusCode);
        mesadaRepository.Verify(r => r.RemoverAsync(It.IsAny<int>()), Times.Never);
    }
}
