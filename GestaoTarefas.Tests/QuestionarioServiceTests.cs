using System.Net;
using GestaoTarefas.Application.DTOs.Questionario;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;
using Moq;
using Xunit;

namespace GestaoTarefas.Tests;

public class QuestionarioServiceTests
{
    private static QuestionarioService CriarServico(
        Mock<IBancoQuestionarioRepository> bancoRepository,
        Mock<IRespostaQuestionarioRepository>? respostaRepository = null,
        PerfilUsuarioEnum perfil = PerfilUsuarioEnum.Filho,
        int usuarioId = 10)
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.Perfil).Returns(perfil);
        currentUser.Setup(c => c.UsuarioId).Returns(usuarioId);

        return new QuestionarioService(
            bancoRepository.Object,
            (respostaRepository ?? new Mock<IRespostaQuestionarioRepository>()).Object,
            currentUser.Object);
    }

    private static PerguntaQuestionario CriarPergunta(string id = "665f1c2b9a1e4b0012345678", int pontos = 10)
    {
        return new PerguntaQuestionario
        {
            Id = id,
            Categoria = "EducacaoFinanceira",
            Dificuldade = "Facil",
            Pergunta = "Você recebeu R$ 20 de mesada. O que seria uma boa escolha?",
            Opcoes = new List<string> { "Gastar tudo", "Guardar uma parte", "Pedir mais", "Comprar sem pensar" },
            RespostaCorreta = 1,
            Explicacao = "Guardar uma parte ajuda a alcançar objetivos maiores.",
            Pontos = pontos,
            Ativa = true
        };
    }

    [Fact]
    public async Task IniciarAsync_SemQuantidade_SorteiaQuantidadePadrao()
    {
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterAleatoriasAsync(It.IsAny<int>())).ReturnsAsync(new[] { CriarPergunta() });

        var servico = CriarServico(repositorio);

        var resultado = await servico.IniciarAsync(null);

        Assert.True(resultado.Sucesso);
        repositorio.Verify(r => r.ObterAleatoriasAsync(QuestionarioService.QuantidadePadrao), Times.Once);
    }

    [Fact]
    public async Task IniciarAsync_QuantidadeAcimaDoMaximo_LimitaAoMaximo()
    {
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterAleatoriasAsync(It.IsAny<int>())).ReturnsAsync(new[] { CriarPergunta() });

        var servico = CriarServico(repositorio);

        await servico.IniciarAsync(500);

        repositorio.Verify(r => r.ObterAleatoriasAsync(QuestionarioService.QuantidadeMaxima), Times.Once);
    }

    [Fact]
    public async Task IniciarAsync_DescartaPerguntasMalFormadas()
    {
        var malFormada = CriarPergunta("665f1c2b9a1e4b0012345679");
        malFormada.RespostaCorreta = 7;

        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterAleatoriasAsync(It.IsAny<int>())).ReturnsAsync(new[] { CriarPergunta(), malFormada });

        var servico = CriarServico(repositorio);

        var resultado = await servico.IniciarAsync(10);

        Assert.True(resultado.Sucesso);
        Assert.Single(resultado.ObjetoRetorno!);
    }

    [Fact]
    public async Task IniciarAsync_SemPerguntas_RetornaNotFound()
    {
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterAleatoriasAsync(It.IsAny<int>())).ReturnsAsync(Array.Empty<PerguntaQuestionario>());

        var servico = CriarServico(repositorio);

        var resultado = await servico.IniciarAsync(10);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.NotFound, resultado.StatusCode);
    }

    [Fact]
    public async Task ResponderAsync_RespostaCorreta_RetornaPontosDaPergunta()
    {
        var pergunta = CriarPergunta(pontos: 15);
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterPorIdAsync(pergunta.Id)).ReturnsAsync(pergunta);

        var servico = CriarServico(repositorio);

        var resultado = await servico.ResponderAsync(new ResponderPerguntaDto { PerguntaId = pergunta.Id, RespostaEscolhida = 1 });

        Assert.True(resultado.Sucesso);
        Assert.True(resultado.ObjetoRetorno!.Correta);
        Assert.Equal(15, resultado.ObjetoRetorno.PontosGanhos);
        Assert.Equal(pergunta.Explicacao, resultado.ObjetoRetorno.Explicacao);
    }

    [Fact]
    public async Task ResponderAsync_RespostaIncorreta_NaoGanhaPontosEInformaCorreta()
    {
        var pergunta = CriarPergunta();
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterPorIdAsync(pergunta.Id)).ReturnsAsync(pergunta);

        var servico = CriarServico(repositorio);

        var resultado = await servico.ResponderAsync(new ResponderPerguntaDto { PerguntaId = pergunta.Id, RespostaEscolhida = 0 });

        Assert.True(resultado.Sucesso);
        Assert.False(resultado.ObjetoRetorno!.Correta);
        Assert.Equal(0, resultado.ObjetoRetorno.PontosGanhos);
        Assert.Equal(1, resultado.ObjetoRetorno.RespostaCorreta);
        Assert.Equal("Guardar uma parte", resultado.ObjetoRetorno.TextoRespostaCorreta);
    }

    [Fact]
    public async Task ResponderAsync_PerguntaSemPontos_UsaPontuacaoPadrao()
    {
        var pergunta = CriarPergunta(pontos: 0);
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterPorIdAsync(pergunta.Id)).ReturnsAsync(pergunta);

        var servico = CriarServico(repositorio);

        var resultado = await servico.ResponderAsync(new ResponderPerguntaDto { PerguntaId = pergunta.Id, RespostaEscolhida = 1 });

        Assert.Equal(PerguntaQuestionario.PontosPadrao, resultado.ObjetoRetorno!.PontosGanhos);
    }

    [Fact]
    public async Task ResponderAsync_AlternativaForaDaLista_RetornaBadRequest()
    {
        var pergunta = CriarPergunta();
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterPorIdAsync(pergunta.Id)).ReturnsAsync(pergunta);

        var servico = CriarServico(repositorio);

        var resultado = await servico.ResponderAsync(new ResponderPerguntaDto { PerguntaId = pergunta.Id, RespostaEscolhida = 4 });

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.BadRequest, resultado.StatusCode);
    }

    [Fact]
    public async Task ResponderAsync_PerguntaInativa_RetornaNotFound()
    {
        var pergunta = CriarPergunta();
        pergunta.Ativa = false;
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterPorIdAsync(pergunta.Id)).ReturnsAsync(pergunta);

        var servico = CriarServico(repositorio);

        var resultado = await servico.ResponderAsync(new ResponderPerguntaDto { PerguntaId = pergunta.Id, RespostaEscolhida = 1 });

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.NotFound, resultado.StatusCode);
    }

    [Fact]
    public async Task ResponderAsync_QuandoFilhoResponde_RegistraNoHistorico()
    {
        var pergunta = CriarPergunta();
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterPorIdAsync(pergunta.Id)).ReturnsAsync(pergunta);
        var respostaRepository = new Mock<IRespostaQuestionarioRepository>();

        var servico = CriarServico(repositorio, respostaRepository, PerfilUsuarioEnum.Filho, usuarioId: 42);

        await servico.ResponderAsync(new ResponderPerguntaDto { PerguntaId = pergunta.Id, RespostaEscolhida = 0 });

        respostaRepository.Verify(r => r.RegistrarAsync(It.Is<RespostaQuestionario>(x =>
            x.FilhoId == 42
            && !x.Correta
            && x.RespostaEscolhida == "Gastar tudo"
            && x.RespostaCorreta == "Guardar uma parte"
            && x.Categoria == "EducacaoFinanceira")), Times.Once);
    }

    [Fact]
    public async Task ResponderAsync_QuandoPaiResponde_NaoRegistraNoHistorico()
    {
        var pergunta = CriarPergunta();
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterPorIdAsync(pergunta.Id)).ReturnsAsync(pergunta);
        var respostaRepository = new Mock<IRespostaQuestionarioRepository>();

        var servico = CriarServico(repositorio, respostaRepository, PerfilUsuarioEnum.Pai);

        var resultado = await servico.ResponderAsync(new ResponderPerguntaDto { PerguntaId = pergunta.Id, RespostaEscolhida = 1 });

        Assert.True(resultado.Sucesso);
        respostaRepository.Verify(r => r.RegistrarAsync(It.IsAny<RespostaQuestionario>()), Times.Never);
    }

    [Fact]
    public async Task ResponderAsync_AlternativaInvalida_NaoRegistraNoHistorico()
    {
        var pergunta = CriarPergunta();
        var repositorio = new Mock<IBancoQuestionarioRepository>();
        repositorio.Setup(r => r.ObterPorIdAsync(pergunta.Id)).ReturnsAsync(pergunta);
        var respostaRepository = new Mock<IRespostaQuestionarioRepository>();

        var servico = CriarServico(repositorio, respostaRepository);

        await servico.ResponderAsync(new ResponderPerguntaDto { PerguntaId = pergunta.Id, RespostaEscolhida = 9 });

        respostaRepository.Verify(r => r.RegistrarAsync(It.IsAny<RespostaQuestionario>()), Times.Never);
    }
}
