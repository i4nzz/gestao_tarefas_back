using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;
using Moq;
using Xunit;

namespace GestaoTarefas.Tests;

public class TarefaServiceTests
{
    private static TarefaService CriarServico(
        Mock<ITarefaRepository> tarefaRepository,
        Mock<IAutorizacaoFamiliarService> autorizacao,
        Mock<IUsuarioRepository>? usuarioRepository = null)
    {
        return new TarefaService(
            tarefaRepository.Object,
            (usuarioRepository ?? new Mock<IUsuarioRepository>()).Object,
            autorizacao.Object);
    }

    private static Tarefa CriarTarefa(DateTime prazo, params ComprovacaoTarefa[] comprovacoes)
    {
        return new Tarefa
        {
            TarefaId = 1,
            FilhoId = 10,
            Titulo = "Lavar louça",
            Pontos = 10,
            Prazo = prazo,
            Comprovacoes = comprovacoes
        };
    }

    private static async Task<StatusTarefaEnum?> ObterStatusAsync(Tarefa tarefa)
    {
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(r => r.ObterPorIdAsync(tarefa.TarefaId)).ReturnsAsync(tarefa);

        var autorizacao = new Mock<IAutorizacaoFamiliarService>();
        autorizacao.Setup(a => a.PodeAcessarFilhoAsync(tarefa.FilhoId)).ReturnsAsync(true);

        var servico = CriarServico(tarefaRepository, autorizacao);

        var resultado = await servico.ObterPorIdAsync(tarefa.TarefaId);

        return resultado.ObjetoRetorno?.Status;
    }

    [Fact]
    public async Task ObterPorIdAsync_SemComprovacaoEPrazoFuturo_RetornaPendente()
    {
        var tarefa = CriarTarefa(DateTime.UtcNow.AddDays(1));

        Assert.Equal(StatusTarefaEnum.Pendente, await ObterStatusAsync(tarefa));
    }

    [Fact]
    public async Task ObterPorIdAsync_SemComprovacaoEPrazoPassado_RetornaExpirada()
    {
        var tarefa = CriarTarefa(DateTime.UtcNow.AddDays(-1));

        Assert.Equal(StatusTarefaEnum.Expirada, await ObterStatusAsync(tarefa));
    }

    [Fact]
    public async Task ObterPorIdAsync_ComComprovacaoPendente_RetornaAguardandoValidacao()
    {
        var comprovacao = new ComprovacaoTarefa(1, "foto.jpg");
        var tarefa = CriarTarefa(DateTime.UtcNow.AddDays(-1), comprovacao);

        Assert.Equal(StatusTarefaEnum.AguardandoValidacao, await ObterStatusAsync(tarefa));
    }

    [Fact]
    public async Task ObterPorIdAsync_ComComprovacaoAprovada_RetornaConcluida()
    {
        var comprovacao = new ComprovacaoTarefa(1, "foto.jpg");
        comprovacao.Aprovar();
        var tarefa = CriarTarefa(DateTime.UtcNow.AddDays(-1), comprovacao);

        Assert.Equal(StatusTarefaEnum.Concluida, await ObterStatusAsync(tarefa));
    }

    [Fact]
    public async Task ObterPorIdAsync_ComComprovacaoReprovadaEPrazoPassado_RetornaExpirada()
    {
        var comprovacao = new ComprovacaoTarefa(1, "foto.jpg");
        comprovacao.Reprovar();
        var tarefa = CriarTarefa(DateTime.UtcNow.AddDays(-1), comprovacao);

        Assert.Equal(StatusTarefaEnum.Expirada, await ObterStatusAsync(tarefa));
    }

    [Fact]
    public async Task ObterPorIdAsync_ComComprovacaoReprovadaEPrazoFuturo_RetornaPendente()
    {
        var comprovacao = new ComprovacaoTarefa(1, "foto.jpg");
        comprovacao.Reprovar();
        var tarefa = CriarTarefa(DateTime.UtcNow.AddDays(1), comprovacao);

        Assert.Equal(StatusTarefaEnum.Pendente, await ObterStatusAsync(tarefa));
    }
}
