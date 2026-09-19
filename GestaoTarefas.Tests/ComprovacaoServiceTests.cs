using System.Net;
using GestaoTarefas.Application.DTOs.Recompensa;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GestaoTarefas.Tests;

public class ComprovacaoServiceTests
{
    private static ComprovacaoService CriarServico(
        Mock<ITarefaRepository> tarefaRepository,
        Mock<IAutorizacaoFamiliarService> autorizacao,
        Mock<IComprovacaoRepository>? comprovacaoRepository = null,
        Mock<IPontuacaoRepository>? pontuacaoRepository = null,
        Mock<IFileStorageService>? fileStorageService = null,
        Mock<IUsuarioRepository>? usuarioRepository = null,
        Mock<IEmailService>? emailService = null)
    {
        return new ComprovacaoService(
            (comprovacaoRepository ?? new Mock<IComprovacaoRepository>()).Object,
            (pontuacaoRepository ?? new Mock<IPontuacaoRepository>()).Object,
            tarefaRepository.Object,
            (usuarioRepository ?? new Mock<IUsuarioRepository>()).Object,
            (emailService ?? new Mock<IEmailService>()).Object,
            (fileStorageService ?? new Mock<IFileStorageService>()).Object,
            autorizacao.Object);
    }

    [Fact]
    public async Task EnviarAsync_QuandoChamadorNaoTemVinculoComFilhoDaTarefa_RetornaForbiddenENaoSalvaArquivo()
    {
        var tarefa = new Tarefa { TarefaId = 5, FilhoId = 20, Titulo = "Lavar louça", Pontos = 10 };
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(r => r.ObterPorIdAsync(5)).ReturnsAsync(tarefa);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(20)).ReturnsAsync(false);

        var fileStorageService = new Mock<IFileStorageService>();
        var comprovacaoRepository = new Mock<IComprovacaoRepository>();

        var servico = CriarServico(tarefaRepository, autorizacao, comprovacaoRepository, fileStorageService: fileStorageService);

        var foto = new Mock<IFormFile>();
        foto.Setup(f => f.Length).Returns(100);

        var dto = new CriarComprovacaoDto { TarefaId = 5, Foto = foto.Object };
        var resultado = await servico.EnviarAsync(dto);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Forbidden, resultado.StatusCode);
        fileStorageService.Verify(f => f.SalvarArquivoAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
        comprovacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<ComprovacaoTarefa>()), Times.Never);
    }

    [Fact]
    public async Task EnviarAsync_QuandoComprovacaoEnviadaComSucesso_NotificaPaisVinculadosPorEmail()
    {
        var filho = new Filho("Léo", "leo@teste.com", "hash", DateTime.UtcNow.AddYears(-10));
        var tarefa = new Tarefa { TarefaId = 5, FilhoId = 20, Titulo = "Lavar louça", Pontos = 10, Filho = filho };
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(r => r.ObterPorIdAsync(5)).ReturnsAsync(tarefa);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(20)).ReturnsAsync(true);

        var fileStorageService = new Mock<IFileStorageService>();
        fileStorageService.Setup(f => f.SalvarArquivoAsync(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("Comprovacoes/foto.jpg");

        var comprovacaoRepository = new Mock<IComprovacaoRepository>();

        var pai1 = new Pai("Ana", "ana@teste.com", "hash");
        var pai2 = new Pai("Bruno", "bruno@teste.com", "hash");
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.ObterPaisPorFilhoIdAsync(20)).ReturnsAsync(new List<Usuario> { pai1, pai2 });

        var emailService = new Mock<IEmailService>();

        var servico = CriarServico(
            tarefaRepository,
            autorizacao,
            comprovacaoRepository,
            fileStorageService: fileStorageService,
            usuarioRepository: usuarioRepository,
            emailService: emailService);

        var foto = new Mock<IFormFile>();
        foto.Setup(f => f.Length).Returns(100);

        var dto = new CriarComprovacaoDto { TarefaId = 5, Foto = foto.Object };
        var resultado = await servico.EnviarAsync(dto);

        Assert.True(resultado.Sucesso);
        comprovacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<ComprovacaoTarefa>()), Times.Once);
        emailService.Verify(e => e.EnviarNotificacaoSistemaAsync("ana@teste.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        emailService.Verify(e => e.EnviarNotificacaoSistemaAsync("bruno@teste.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidarAsync_QuandoPaiNaoTemVinculoComFilhoDaTarefa_RetornaForbiddenENaoCreditaPontos()
    {
        var comprovacao = new ComprovacaoTarefa(5, "Comprovacoes/foto.jpg");
        var comprovacaoRepository = new Mock<IComprovacaoRepository>();
        comprovacaoRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(comprovacao);

        var tarefa = new Tarefa { TarefaId = 5, FilhoId = 20, Titulo = "Lavar louça", Pontos = 10 };
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(r => r.ObterPorIdAsync(5)).ReturnsAsync(tarefa);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(20)).ReturnsAsync(false);

        var pontuacaoRepository = new Mock<IPontuacaoRepository>();

        var servico = CriarServico(tarefaRepository, autorizacao, comprovacaoRepository, pontuacaoRepository);

        var resultado = await servico.ValidarAsync(1, aprovar: true);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Forbidden, resultado.StatusCode);
        pontuacaoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Pontuacao>()), Times.Never);
        comprovacaoRepository.Verify(r => r.AtualizarAsync(It.IsAny<ComprovacaoTarefa>()), Times.Never);
    }
}
