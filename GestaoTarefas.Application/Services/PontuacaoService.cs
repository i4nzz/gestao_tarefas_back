using System.Net;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Pontuacao;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Mapping;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;

namespace GestaoTarefas.Application.Services;

public class PontuacaoService : IPontuacaoService
{
    private readonly IPontuacaoRepository _pontuacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IResgatePontuacaoRepository _resgatePontuacaoRepository;
    private readonly IAutorizacaoFamiliarService _autorizacao;
    public PontuacaoService(
        IPontuacaoRepository pontuacaoRepository
        , IUsuarioRepository usuarioRepository
        , ITarefaRepository tarefaRepository
        , IResgatePontuacaoRepository resgatePontuacaoRepository
        , IAutorizacaoFamiliarService autorizacao
        )
    {
        _pontuacaoRepository = pontuacaoRepository;
        _usuarioRepository = usuarioRepository;
        _tarefaRepository = tarefaRepository;
        _resgatePontuacaoRepository = resgatePontuacaoRepository;
        _autorizacao = autorizacao;
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoPontuacaoDto>>> ObterPorFilhoAsync(int filhoId)
    {
        if (!await _autorizacao.PodeAcessarFilhoAsync(filhoId))
        {
            return new RespostaMetodos<IEnumerable<RetornoPontuacaoDto>>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar as pontuações deste filho"
            };
        }

        var pontuacoes = await _pontuacaoRepository.ObterPorFilhoAsync(filhoId);

        if (pontuacoes == null || !pontuacoes.Any())
        {
            return new RespostaMetodos<IEnumerable<RetornoPontuacaoDto>>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Nenhuma pontuação encontrada"
            };
        }
        var retornoPontuacoes = pontuacoes.Select(p => p.ToDto()).ToList();

        return new RespostaMetodos<IEnumerable<RetornoPontuacaoDto>>
        {
            Sucesso = true,
            ObjetoRetorno = retornoPontuacoes,
            Mensagem = "Pontuações encontradas"
        };
    }

    public async Task<RespostaMetodos<int>> ObterTotalPontosAsync(int filhoId)
    {
        if (!await _autorizacao.PodeAcessarFilhoAsync(filhoId))
        {
            return new RespostaMetodos<int>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar o total de pontos deste filho"
            };
        }

        var ganhos = await _pontuacaoRepository.ObterTotalPontosAsync(filhoId);
        var resgates = await _resgatePontuacaoRepository.ObterTotalResgatesAsync(filhoId);
        var saldo = ganhos - resgates;

        return new RespostaMetodos<int>
        {
            Sucesso = true,
            ObjetoRetorno = saldo,
            Mensagem = $"Pontos encontrados para o filhoId {filhoId}"
        };
    }

    public async Task<RespostaMetodos<RetornoPontuacaoDto>> AdicionarAsync(CriarPontuacaoDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(dto.FilhoId);

        if (usuario == null)
        {
            return new RespostaMetodos<RetornoPontuacaoDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Usuário não encontrado"
            };
        }

        var tarefa = await _tarefaRepository.ObterPorIdAsync(dto.TarefaId);

        if (tarefa == null)
        {
            return new RespostaMetodos<RetornoPontuacaoDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Tarefa não encontrada"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(dto.FilhoId))
        {
            return new RespostaMetodos<RetornoPontuacaoDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não pode adicionar pontos para um filho que não é vinculado a você"
            };
        }

        if (tarefa.FilhoId != dto.FilhoId)
        {
            return new RespostaMetodos<RetornoPontuacaoDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "A tarefa informada não pertence ao filho informado"
            };
        }

        if (dto.Pontos <= 0)
        {
            return new RespostaMetodos<RetornoPontuacaoDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Os pontos devem ser maiores que zero"
            };
        }

        var pontuacao = Pontuacao.CriarGanho(dto.FilhoId, dto.TarefaId, dto.Pontos);

        await _pontuacaoRepository.AdicionarAsync(pontuacao);

        var retornoPontuacao = pontuacao.ToDto();

        return new RespostaMetodos<RetornoPontuacaoDto>
        {
            Sucesso = true,
            ObjetoRetorno = retornoPontuacao,
            Mensagem = "Pontuação adicionada com sucesso"
        };
    }
}
