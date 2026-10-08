using System.Net;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;
using Moq;
using Xunit;

namespace GestaoTarefas.Tests;

public class RelatorioServiceTests
{
    private const int FilhoId = 10;

    private static RelatorioService CriarServico(
        IEnumerable<Tarefa>? tarefas = null,
        IEnumerable<Pontuacao>? pontuacoes = null,
        IEnumerable<RespostaQuestionario>? respostas = null,
        bool autorizado = true)
    {
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(r => r.ObterPorFilhoAsync(FilhoId)).ReturnsAsync(tarefas ?? new List<Tarefa>());

        var pontuacaoRepository = new Mock<IPontuacaoRepository>();
        pontuacaoRepository.Setup(r => r.ObterPorFilhoAsync(FilhoId)).ReturnsAsync(pontuacoes ?? new List<Pontuacao>());

        // O repositório real filtra por data; aqui o filtro é reproduzido para os testes de período.
        var lista = (respostas ?? new List<RespostaQuestionario>()).OrderByDescending(r => r.DataResposta).ToList();
        var respostaRepository = new Mock<IRespostaQuestionarioRepository>();
        respostaRepository
            .Setup(r => r.ObterPorFilhoAsync(FilhoId, It.IsAny<DateTime?>()))
            .ReturnsAsync((int _, DateTime? desde) => lista.Where(r => !desde.HasValue || r.DataResposta >= desde));

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(FilhoId)).ReturnsAsync(autorizado);

        return new RelatorioService(
            tarefaRepository.Object,
            pontuacaoRepository.Object,
            respostaRepository.Object,
            autorizacao.Object);
    }

    private static Tarefa CriarTarefa(DateTime prazo, params StatusValidacaoTarefaEnum[] comprovacoes)
    {
        var tarefa = new Tarefa { FilhoId = FilhoId, Titulo = "Arrumar a cama", Pontos = 10, Prazo = prazo };
        foreach (var status in comprovacoes)
        {
            var comprovacao = new ComprovacaoTarefa(1, "img");
            if (status == StatusValidacaoTarefaEnum.Aprovada) comprovacao.Aprovar();
            if (status == StatusValidacaoTarefaEnum.Reprovada) comprovacao.Reprovar();
            tarefa.Comprovacoes.Add(comprovacao);
        }
        return tarefa;
    }

    private static RespostaQuestionario CriarResposta(string categoria, bool correta, DateTime data)
    {
        return new RespostaQuestionario
        {
            FilhoId = FilhoId,
            Categoria = categoria,
            Pergunta = "Pergunta",
            Correta = correta,
            DataResposta = data
        };
    }

    [Fact]
    public async Task ObterPorFilhoAsync_SemPermissao_RetornaForbidden()
    {
        var servico = CriarServico(autorizado: false);

        var resultado = await servico.ObterPorFilhoAsync(FilhoId, 30);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.Forbidden, resultado.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(366)]
    public async Task ObterPorFilhoAsync_PeriodoInvalido_RetornaBadRequest(int dias)
    {
        var servico = CriarServico();

        var resultado = await servico.ObterPorFilhoAsync(FilhoId, dias);

        Assert.False(resultado.Sucesso);
        Assert.Equal(HttpStatusCode.BadRequest, resultado.StatusCode);
    }

    [Fact]
    public async Task ObterPorFilhoAsync_ContaTarefasPorStatus()
    {
        var agora = DateTime.UtcNow;
        var tarefas = new List<Tarefa>
        {
            CriarTarefa(agora.AddDays(-2), StatusValidacaoTarefaEnum.Aprovada),
            CriarTarefa(agora.AddDays(-1), StatusValidacaoTarefaEnum.Reprovada, StatusValidacaoTarefaEnum.Aprovada),
            CriarTarefa(agora.AddDays(2), StatusValidacaoTarefaEnum.Pendente),
            CriarTarefa(agora.AddDays(3)),
            CriarTarefa(agora.AddDays(-3), StatusValidacaoTarefaEnum.Reprovada),
        };
        var servico = CriarServico(tarefas: tarefas);

        var resultado = await servico.ObterPorFilhoAsync(FilhoId, null);
        var resumo = resultado.ObjetoRetorno!.Tarefas;

        Assert.Equal(5, resumo.Total);
        Assert.Equal(2, resumo.Concluidas);
        Assert.Equal(1, resumo.AguardandoValidacao);
        Assert.Equal(1, resumo.Pendentes);
        Assert.Equal(1, resumo.Expiradas);
        Assert.Equal(40, resumo.PercentualConcluidas);
        Assert.Equal(2, resumo.ComprovacoesAprovadas);
        Assert.Equal(2, resumo.ComprovacoesReprovadas);
    }

    [Fact]
    public async Task ObterPorFilhoAsync_ComPeriodo_IgnoraTarefasComPrazoAntigo()
    {
        var agora = DateTime.UtcNow;
        var tarefas = new List<Tarefa>
        {
            CriarTarefa(agora.AddDays(-60), StatusValidacaoTarefaEnum.Aprovada),
            CriarTarefa(agora.AddDays(-2)),
        };
        var servico = CriarServico(tarefas: tarefas);

        var resultado = await servico.ObterPorFilhoAsync(FilhoId, 7);

        Assert.Equal(1, resultado.ObjetoRetorno!.Tarefas.Total);
        Assert.Equal(1, resultado.ObjetoRetorno.Tarefas.Expiradas);
    }

    [Fact]
    public async Task ObterPorFilhoAsync_ResumeQuestionarioPorCategoria()
    {
        var agora = DateTime.UtcNow;
        var respostas = new List<RespostaQuestionario>
        {
            CriarResposta("EducacaoFinanceira", true, agora.AddMinutes(-1)),
            CriarResposta("EducacaoFinanceira", true, agora.AddMinutes(-2)),
            CriarResposta("EducacaoFinanceira", false, agora.AddMinutes(-3)),
            CriarResposta("TarefasDomesticas", false, agora.AddMinutes(-4)),
        };
        var servico = CriarServico(respostas: respostas);

        var resultado = await servico.ObterPorFilhoAsync(FilhoId, 30);
        var questionario = resultado.ObjetoRetorno!.Questionario;

        Assert.Equal(4, questionario.TotalRespostas);
        Assert.Equal(2, questionario.Acertos);
        Assert.Equal(2, questionario.Erros);
        Assert.Equal(50, questionario.PercentualAcerto);
        // A categoria com menor aproveitamento vem primeiro: é a que pede mais atenção.
        Assert.Equal("TarefasDomesticas", questionario.PorCategoria[0].Categoria);
        Assert.Equal(67, questionario.PorCategoria[1].PercentualAcerto);
        Assert.Equal(4, questionario.UltimasRespostas.Count);
        Assert.Equal(7, questionario.UltimosSeteDias.Count);
        Assert.Equal(4, questionario.UltimosSeteDias.Sum(d => d.Acertos + d.Erros));
    }

    [Fact]
    public async Task ObterPorFilhoAsync_PeriodoCurto_AindaMostraAtividadeDosUltimosSeteDias()
    {
        var agora = DateTime.UtcNow;
        var respostas = new List<RespostaQuestionario>
        {
            CriarResposta("Responsabilidade", true, agora.AddMinutes(-5)),
            CriarResposta("Responsabilidade", false, agora.AddDays(-4)),
        };
        var servico = CriarServico(respostas: respostas);

        var resultado = await servico.ObterPorFilhoAsync(FilhoId, 1);
        var questionario = resultado.ObjetoRetorno!.Questionario;

        Assert.Equal(1, questionario.TotalRespostas);
        Assert.Equal(2, questionario.UltimosSeteDias.Sum(d => d.Acertos + d.Erros));
    }

    [Fact]
    public async Task ObterPorFilhoAsync_SomaSoPontosDeGanhoNoPeriodo()
    {
        var agora = DateTime.UtcNow;
        var pontuacoes = new List<Pontuacao>
        {
            new() { FilhoId = FilhoId, Pontos = 10, Tipo = TipoPontuacaoEnum.Ganho, DataRegistro = agora.AddDays(-1) },
            new() { FilhoId = FilhoId, Pontos = 20, Tipo = TipoPontuacaoEnum.Ganho, DataRegistro = agora.AddDays(-40) },
        };
        var servico = CriarServico(pontuacoes: pontuacoes);

        var resultado = await servico.ObterPorFilhoAsync(FilhoId, 30);

        Assert.Equal(10, resultado.ObjetoRetorno!.Tarefas.PontosGanhos);
    }
}
