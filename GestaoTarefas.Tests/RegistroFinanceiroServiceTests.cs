using GestaoTarefas.Application.DTOs.RegistroFinanceiro;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;
using Moq;
using Xunit;

namespace GestaoTarefas.Tests;

public class RegistroFinanceiroServiceTests
{
    private static RegistroFinanceiroService CriarServico(
        Mock<IUsuarioRepository> usuarioRepository,
        Mock<ICategoriaFinanceiraRepository> categoriaRepository,
        Mock<IMesadaRepository> mesadaRepository,
        Mock<IAutorizacaoFamiliarService> autorizacao,
        Mock<IRegistroFinanceiroRepository>? registroRepository = null)
    {
        return new RegistroFinanceiroService(
            (registroRepository ?? new Mock<IRegistroFinanceiroRepository>()).Object,
            usuarioRepository.Object,
            categoriaRepository.Object,
            mesadaRepository.Object,
            autorizacao.Object);
    }

    [Fact]
    public async Task CriarAsync_QuandoMesadaPertenceAOutroFilho_NaoRegistraGasto()
    {
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(new Usuario("Filho A", "a@teste.com", "hash", PerfilUsuarioEnum.Filho));

        var categoriaRepository = new Mock<ICategoriaFinanceiraRepository>();
        categoriaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new CategoriaFinanceira { CategoriaFinanceiraId = 1, Nome = "Lazer" });

        var mesadaDeOutroFilho = new Mesada { MesadaId = 5, FilhoId = 20, Valor = 100, Mes = 8, Ano = 2026 };
        var mesadaRepository = new Mock<IMesadaRepository>();
        mesadaRepository.Setup(r => r.ObterPorIdAsync(5)).ReturnsAsync(mesadaDeOutroFilho);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var registroRepository = new Mock<IRegistroFinanceiroRepository>();
        var servico = CriarServico(usuarioRepository, categoriaRepository, mesadaRepository, autorizacao, registroRepository);

        var dto = new CriarRegistroFinanceiroDto { FilhoId = 10, CategoriaId = 1, MesadaId = 5, Descricao = "Cinema", Valor = 30 };
        var resultado = await servico.CriarAsync(dto);

        Assert.False(resultado.Sucesso);
        registroRepository.Verify(r => r.AdicionarAsync(It.IsAny<RegistroFinanceiro>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_QuandoMesadaPertenceAoFilhoCorreto_RegistraGasto()
    {
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(new Usuario("Filho A", "a@teste.com", "hash", PerfilUsuarioEnum.Filho));

        var categoriaRepository = new Mock<ICategoriaFinanceiraRepository>();
        categoriaRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new CategoriaFinanceira { CategoriaFinanceiraId = 1, Nome = "Lazer" });

        var mesadaDoFilho = new Mesada { MesadaId = 5, FilhoId = 10, Valor = 100, Mes = 8, Ano = 2026 };
        var mesadaRepository = new Mock<IMesadaRepository>();
        mesadaRepository.Setup(r => r.ObterPorIdAsync(5)).ReturnsAsync(mesadaDoFilho);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var registroRepository = new Mock<IRegistroFinanceiroRepository>();
        var servico = CriarServico(usuarioRepository, categoriaRepository, mesadaRepository, autorizacao, registroRepository);

        var dto = new CriarRegistroFinanceiroDto { FilhoId = 10, CategoriaId = 1, MesadaId = 5, Descricao = "Cinema", Valor = 30 };
        var resultado = await servico.CriarAsync(dto);

        Assert.True(resultado.Sucesso);
        registroRepository.Verify(r => r.AdicionarAsync(It.IsAny<RegistroFinanceiro>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_QuandoFilhoNaoVinculado_RetornaForbidden()
    {
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync(new Usuario("Filho X", "x@teste.com", "hash", PerfilUsuarioEnum.Filho));

        var categoriaRepository = new Mock<ICategoriaFinanceiraRepository>();
        var mesadaRepository = new Mock<IMesadaRepository>();

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(99)).ReturnsAsync(false);

        var registroRepository = new Mock<IRegistroFinanceiroRepository>();
        var servico = CriarServico(usuarioRepository, categoriaRepository, mesadaRepository, autorizacao, registroRepository);

        var dto = new CriarRegistroFinanceiroDto { FilhoId = 99, CategoriaId = 1, MesadaId = 5, Descricao = "Cinema", Valor = 30 };
        var resultado = await servico.CriarAsync(dto);

        Assert.False(resultado.Sucesso);
        registroRepository.Verify(r => r.AdicionarAsync(It.IsAny<RegistroFinanceiro>()), Times.Never);
    }
}
