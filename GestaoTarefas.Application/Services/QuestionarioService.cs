using System.Net;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Questionario;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Mapping;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;

namespace GestaoTarefas.Application.Services;

/// <summary>
/// Regras do questionário gamificado: sorteio das perguntas de uma partida e validação das
/// respostas. Os pontos do questionário valem só dentro do jogo e não entram no saldo de
/// pontos usado para resgatar recompensas. As respostas dos filhos ficam registradas para o
/// acompanhamento dos pais.
/// </summary>
public class QuestionarioService : IQuestionarioService
{
    public const int QuantidadePadrao = 10;
    public const int QuantidadeMaxima = 20;

    private readonly IBancoQuestionarioRepository _bancoQuestionarioRepository;
    private readonly IRespostaQuestionarioRepository _respostaQuestionarioRepository;
    private readonly ICurrentUserService _currentUser;

    public QuestionarioService(
        IBancoQuestionarioRepository bancoQuestionarioRepository
        , IRespostaQuestionarioRepository respostaQuestionarioRepository
        , ICurrentUserService currentUser
        )
    {
        _bancoQuestionarioRepository = bancoQuestionarioRepository;
        _respostaQuestionarioRepository = respostaQuestionarioRepository;
        _currentUser = currentUser;
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoPerguntaQuestionarioDto>>> IniciarAsync(int? quantidade)
    {
        var quantidadeSorteio = Math.Clamp(quantidade ?? QuantidadePadrao, 1, QuantidadeMaxima);

        // Documento mal formado (sem alternativas ou com resposta fora da lista) fica fora da partida.
        var perguntas = (await _bancoQuestionarioRepository.ObterAleatoriasAsync(quantidadeSorteio))
            .Where(p => p.EstaBemFormada)
            .Select(p => p.ToDto())
            .ToList();

        if (!perguntas.Any())
        {
            return new RespostaMetodos<IEnumerable<RetornoPerguntaQuestionarioDto>>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.NotFound,
                Mensagem = "Nenhuma pergunta disponível no momento"
            };
        }

        return new RespostaMetodos<IEnumerable<RetornoPerguntaQuestionarioDto>>
        {
            Sucesso = true,
            StatusCode = HttpStatusCode.OK,
            ObjetoRetorno = perguntas,
            Mensagem = "Perguntas sorteadas"
        };
    }

    public async Task<RespostaMetodos<RetornoRespostaQuestionarioDto>> ResponderAsync(ResponderPerguntaDto dto)
    {
        var pergunta = await _bancoQuestionarioRepository.ObterPorIdAsync(dto.PerguntaId);

        if (pergunta == null || !pergunta.Ativa || !pergunta.EstaBemFormada)
        {
            return new RespostaMetodos<RetornoRespostaQuestionarioDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.NotFound,
                Mensagem = "Pergunta não encontrada"
            };
        }

        if (!pergunta.RespostaEhValida(dto.RespostaEscolhida))
        {
            return new RespostaMetodos<RetornoRespostaQuestionarioDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.BadRequest,
                Mensagem = "Alternativa inválida"
            };
        }

        var correta = pergunta.EstaCorreta(dto.RespostaEscolhida);
        // Errar não tira pontos: o objetivo é ensinar, não punir.
        var pontosGanhos = correta ? pergunta.PontosPorAcerto : 0;

        // Só a partida do próprio filho entra no histórico; um pai testando o jogo não conta.
        if (_currentUser.Perfil == PerfilUsuarioEnum.Filho)
        {
            await _respostaQuestionarioRepository.RegistrarAsync(new RespostaQuestionario
            {
                FilhoId = _currentUser.UsuarioId,
                PerguntaId = pergunta.Id,
                Categoria = pergunta.Categoria,
                Dificuldade = pergunta.Dificuldade,
                Pergunta = pergunta.Pergunta,
                RespostaEscolhida = pergunta.Opcoes[dto.RespostaEscolhida],
                RespostaCorreta = pergunta.Opcoes[pergunta.RespostaCorreta],
                Correta = correta,
                PontosGanhos = pontosGanhos,
                DataResposta = DateTime.UtcNow
            });
        }

        return new RespostaMetodos<RetornoRespostaQuestionarioDto>
        {
            Sucesso = true,
            StatusCode = HttpStatusCode.OK,
            ObjetoRetorno = new RetornoRespostaQuestionarioDto
            {
                PerguntaId = pergunta.Id,
                Categoria = pergunta.Categoria,
                Correta = correta,
                RespostaEscolhida = dto.RespostaEscolhida,
                RespostaCorreta = pergunta.RespostaCorreta,
                TextoRespostaCorreta = pergunta.Opcoes[pergunta.RespostaCorreta],
                Explicacao = pergunta.Explicacao,
                PontosGanhos = pontosGanhos
            },
            Mensagem = correta ? "Resposta correta" : "Resposta incorreta"
        };
    }
}
