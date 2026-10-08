using System.Net;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Relatorio;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Helper;
using GestaoTarefas.Domain.Interfaces;

namespace GestaoTarefas.Application.Services;

/// <summary>
/// Monta o acompanhamento de um filho para o responsável. Tudo é calculado na hora a partir
/// das tarefas, das pontuações e do histórico de respostas do questionário, então cada
/// resposta do filho já aparece na próxima consulta.
/// </summary>
public class RelatorioService : IRelatorioService
{
    public const int DiasMaximo = 365;
    public const int QuantidadeUltimasRespostas = 10;
    private const int DiasDaAtividadeRecente = 7;

    private readonly ITarefaRepository _tarefaRepository;
    private readonly IPontuacaoRepository _pontuacaoRepository;
    private readonly IRespostaQuestionarioRepository _respostaQuestionarioRepository;
    private readonly IAutorizacaoFamiliarService _autorizacao;

    public RelatorioService(
        ITarefaRepository tarefaRepository
        , IPontuacaoRepository pontuacaoRepository
        , IRespostaQuestionarioRepository respostaQuestionarioRepository
        , IAutorizacaoFamiliarService autorizacao
        )
    {
        _tarefaRepository = tarefaRepository;
        _pontuacaoRepository = pontuacaoRepository;
        _respostaQuestionarioRepository = respostaQuestionarioRepository;
        _autorizacao = autorizacao;
    }

    public async Task<RespostaMetodos<RelatorioFilhoDto>> ObterPorFilhoAsync(int filhoId, int? dias)
    {
        if (dias.HasValue && (dias <= 0 || dias > DiasMaximo))
        {
            return new RespostaMetodos<RelatorioFilhoDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.BadRequest,
                Mensagem = $"O período deve ser entre 1 e {DiasMaximo} dias"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(filhoId))
        {
            return new RespostaMetodos<RelatorioFilhoDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar o acompanhamento deste filho"
            };
        }

        var agora = DateTime.UtcNow;
        DateTime? inicio = dias.HasValue ? agora.AddDays(-dias.Value) : null;

        var tarefas = await _tarefaRepository.ObterPorFilhoAsync(filhoId);
        var pontuacoes = await _pontuacaoRepository.ObterPorFilhoAsync(filhoId);

        // A atividade dos últimos 7 dias aparece mesmo com um período menor selecionado.
        var inicioAtividadeRecente = agora.Date.AddDays(-DiasDaAtividadeRecente);
        DateTime? buscarRespostasDesde = inicio.HasValue && inicio > inicioAtividadeRecente ? inicioAtividadeRecente : inicio;
        var respostas = (await _respostaQuestionarioRepository.ObterPorFilhoAsync(filhoId, buscarRespostasDesde)).ToList();

        return new RespostaMetodos<RelatorioFilhoDto>
        {
            Sucesso = true,
            StatusCode = HttpStatusCode.OK,
            ObjetoRetorno = new RelatorioFilhoDto
            {
                FilhoId = filhoId,
                PeriodoDias = dias,
                GeradoEm = agora,
                Tarefas = ResumirTarefas(tarefas, pontuacoes, inicio, agora),
                Questionario = ResumirQuestionario(respostas, inicio, agora)
            },
            Mensagem = "Acompanhamento gerado"
        };
    }

    private static ResumoTarefasRelatorioDto ResumirTarefas(
        IEnumerable<Tarefa> tarefas, IEnumerable<Pontuacao> pontuacoes, DateTime? inicio, DateTime agora)
    {
        // Tarefas com prazo dentro do período (as de prazo futuro também entram, ainda em aberto).
        var doPeriodo = tarefas.Where(t => !inicio.HasValue || t.Prazo >= inicio).ToList();
        var status = doPeriodo.Select(t => t.CalcularStatus(agora)).ToList();
        var comprovacoes = tarefas
            .SelectMany(t => t.Comprovacoes)
            .Where(c => !inicio.HasValue || c.DataEnvio >= inicio)
            .ToList();

        var concluidas = status.Count(s => s == StatusTarefaEnum.Concluida);

        return new ResumoTarefasRelatorioDto
        {
            Total = doPeriodo.Count,
            Concluidas = concluidas,
            AguardandoValidacao = status.Count(s => s == StatusTarefaEnum.AguardandoValidacao),
            Pendentes = status.Count(s => s == StatusTarefaEnum.Pendente),
            Expiradas = status.Count(s => s == StatusTarefaEnum.Expirada),
            PercentualConcluidas = Percentual(concluidas, doPeriodo.Count),
            ComprovacoesAprovadas = comprovacoes.Count(c => c.Status == StatusValidacaoTarefaEnum.Aprovada),
            ComprovacoesReprovadas = comprovacoes.Count(c => c.Status == StatusValidacaoTarefaEnum.Reprovada),
            PontosGanhos = pontuacoes
                .Where(p => p.Tipo == TipoPontuacaoEnum.Ganho && (!inicio.HasValue || p.DataRegistro >= inicio))
                .Sum(p => p.Pontos)
        };
    }

    private static ResumoQuestionarioRelatorioDto ResumirQuestionario(
        List<RespostaQuestionario> respostas, DateTime? inicio, DateTime agora)
    {
        var doPeriodo = respostas.Where(r => !inicio.HasValue || r.DataResposta >= inicio).ToList();
        var acertos = doPeriodo.Count(r => r.Correta);

        var hoje = agora.ToBrazilTime().Date;
        var porDia = respostas
            .GroupBy(r => r.DataResposta.ToBrazilTime().Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        return new ResumoQuestionarioRelatorioDto
        {
            TotalRespostas = doPeriodo.Count,
            Acertos = acertos,
            Erros = doPeriodo.Count - acertos,
            PercentualAcerto = Percentual(acertos, doPeriodo.Count),
            DiasJogados = doPeriodo.Select(r => r.DataResposta.ToBrazilTime().Date).Distinct().Count(),
            UltimaAtividade = respostas.FirstOrDefault()?.DataResposta,
            PorCategoria = doPeriodo
                .GroupBy(r => r.Categoria)
                .Select(g =>
                {
                    var acertosCategoria = g.Count(r => r.Correta);
                    return new DesempenhoCategoriaDto
                    {
                        Categoria = g.Key,
                        Total = g.Count(),
                        Acertos = acertosCategoria,
                        PercentualAcerto = Percentual(acertosCategoria, g.Count())
                    };
                })
                .OrderBy(c => c.PercentualAcerto)
                .ThenByDescending(c => c.Total)
                .ToList(),
            UltimosSeteDias = Enumerable.Range(0, DiasDaAtividadeRecente)
                .Select(i => hoje.AddDays(i - (DiasDaAtividadeRecente - 1)))
                .Select(dia =>
                {
                    var doDia = porDia.TryGetValue(dia, out var lista) ? lista : new List<RespostaQuestionario>();
                    return new AtividadeDiaDto
                    {
                        Data = dia.ToString("yyyy-MM-dd"),
                        Acertos = doDia.Count(r => r.Correta),
                        Erros = doDia.Count(r => !r.Correta)
                    };
                })
                .ToList(),
            UltimasRespostas = doPeriodo
                .Take(QuantidadeUltimasRespostas)
                .Select(r => new RespostaRecenteDto
                {
                    Pergunta = r.Pergunta,
                    Categoria = r.Categoria,
                    RespostaEscolhida = r.RespostaEscolhida,
                    RespostaCorreta = r.RespostaCorreta,
                    Correta = r.Correta,
                    DataResposta = r.DataResposta
                })
                .ToList()
        };
    }

    private static int Percentual(int parte, int total) =>
        total == 0 ? 0 : (int)Math.Round(parte * 100.0 / total);
}
