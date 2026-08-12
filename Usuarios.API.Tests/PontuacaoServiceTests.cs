using GestaoTarefas.Application.DTOs.Pontuacao;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using Moq;
using Xunit;

namespace Usuarios.API.Tests;

public class PontuacaoServiceTests
{
    private static PontuacaoService CriarServico(
        Mock<IUsuarioRepository> usuarioRepository,
        Mock<ITarefaRepository> tarefaRepository,
        Mock<IAutorizacaoFamiliarService> autorizacao,
        Mock<IPontuacaoRepository>? pontuacaoRepository = null)
    {
        return new PontuacaoService(
            (pontuacaoRepository ?? new Mock<IPontuacaoRepository>()).Object,
            usuarioRepository.Object,
            tarefaRepository.Object,
            autorizacao.Object);
    }

    [Fact]
    public async Task AdicionarAsync_QuandoTarefaPertenceAOutroFilho_NaoCreditaPontos()
    {
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(new Usuario("Filho A", "a@teste.com", "hash", GestaoTarefas.Domain.Enum.PerfilUsuarioEnum.Filho));

        var tarefaDeOutroFilho = new Tarefa { TarefaId = 5, FilhoId = 20, Titulo = "Lavar louça", Pontos = 10 };
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(r => r.ObterPorIdAsync(5)).ReturnsAsync(tarefaDeOutroFilho);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var pontuacaoRepository = new Mock<IPontuacaoRepository>();
        var servico = CriarServico(usuarioRepository, tarefaRepository, autorizacao, pontuacaoRepository);

        var dto = new CriarPontuacaoDto { FilhoId = 10, TarefaId = 5, Pontos = 10 };
        var resultado = await servico.AdicionarAsync(dto);

        Assert.False(resultado.Sucesso);
        pontuacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Pontuacao>()), Times.Never);
    }

    [Fact]
    public async Task AdicionarAsync_QuandoTarefaPertenceAoFilhoCorreto_CreditaPontos()
    {
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(new Usuario("Filho A", "a@teste.com", "hash", GestaoTarefas.Domain.Enum.PerfilUsuarioEnum.Filho));

        var tarefaDoFilho = new Tarefa { TarefaId = 5, FilhoId = 10, Titulo = "Lavar louça", Pontos = 10 };
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(r => r.ObterPorIdAsync(5)).ReturnsAsync(tarefaDoFilho);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(10)).ReturnsAsync(true);

        var pontuacaoRepository = new Mock<IPontuacaoRepository>();
        var servico = CriarServico(usuarioRepository, tarefaRepository, autorizacao, pontuacaoRepository);

        var dto = new CriarPontuacaoDto { FilhoId = 10, TarefaId = 5, Pontos = 10 };
        var resultado = await servico.AdicionarAsync(dto);

        Assert.True(resultado.Sucesso);
        pontuacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Pontuacao>()), Times.Once);
    }
}
