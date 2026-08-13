using System.Net;
using System.Transactions;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Recompensa;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Mapping;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;

namespace GestaoTarefas.Application.Services;

public class RecompensaService : IRecompensaService
{
    private readonly IRecompensaRepository _recompensaRepository;
    private readonly IPontuacaoRepository _pontuacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IResgatePontuacaoRepository _resgatePontuacaoRepository;
    private readonly IAutorizacaoFamiliarService _autorizacao;

    public RecompensaService(
        IRecompensaRepository recompensaRepository
        , IPontuacaoRepository pontuacaoRepository
        , IUsuarioRepository usuarioRepository
        , IResgatePontuacaoRepository resgatePontuacaoRepository
        , IAutorizacaoFamiliarService autorizacao
        )
    {
        _recompensaRepository = recompensaRepository;
        _pontuacaoRepository = pontuacaoRepository;
        _usuarioRepository = usuarioRepository;
        _resgatePontuacaoRepository = resgatePontuacaoRepository;
        _autorizacao = autorizacao;
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoRecompensaDto>>> ObterPorFilhoAsync(int filhoId)
    {
        if (!await _autorizacao.PodeAcessarFilhoAsync(filhoId))
        {
            return new RespostaMetodos<IEnumerable<RetornoRecompensaDto>>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar as recompensas deste filho"
            };
        }

        var recompensas = await _recompensaRepository.ObterPorFilhoAsync(filhoId);

        if (recompensas == null || !recompensas.Any())
        {
            return new RespostaMetodos<IEnumerable<RetornoRecompensaDto>>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Nenhuma recompensa encontrada para este filho."
            };
        }

        var retornoRecompensas = recompensas.ToDtoList();

        return new RespostaMetodos<IEnumerable<RetornoRecompensaDto>>
        {
            Sucesso = true,
            ObjetoRetorno = retornoRecompensas,
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Recompensas obtidas com sucesso."
        };
    }

    public async Task<RespostaMetodos<RetornoRecompensaDto?>> ObterPorIdAsync(int id)
    {
        var recompensa = await _recompensaRepository.ObterPorIdAsync(id);

        if (recompensa == null)
        {
            return new RespostaMetodos<RetornoRecompensaDto?>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Recompensa não encontrada."
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(recompensa.FilhoId))
        {
            return new RespostaMetodos<RetornoRecompensaDto?>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar esta recompensa"
            };
        }

        var retornoRecompensa = recompensa.ToDto();

        return new RespostaMetodos<RetornoRecompensaDto?>
        {
            Sucesso = true,
            ObjetoRetorno = retornoRecompensa,
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Recompensa obtida com sucesso."
        };
    }

    public async Task<RespostaMetodos<RetornoRecompensaDto>> CriarAsync(CriarRecompensaDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(dto.FilhoId);

        if (usuario == null)
        {
            return new RespostaMetodos<RetornoRecompensaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Filho não encontrado."
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(dto.FilhoId))
        {
            return new RespostaMetodos<RetornoRecompensaDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não pode criar recompensas para um filho que não é vinculado a você"
            };
        }

        if (dto.PontosNecessarios <= 0)
        {
            return new RespostaMetodos<RetornoRecompensaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Pontos necessários devem ser maiores que zero."
            };
        }

        var recompensa = new Recompensa
        {
            FilhoId = dto.FilhoId,
            Descricao = dto.Descricao,
            PontosNecessarios = dto.PontosNecessarios,
            Ativa = true
        };

        await _recompensaRepository.AdicionarAsync(recompensa);

        var retornoRecompensa = recompensa.ToDto();

        return new RespostaMetodos<RetornoRecompensaDto>
        {
            Sucesso = true,
            ObjetoRetorno = retornoRecompensa,
            StatusCode = HttpStatusCode.Created,
            Mensagem = "Recompensa criada com sucesso."
        };
    }

    public async Task<RespostaMetodos<RetornoRecompensaDto>> AtualizarAsync(int id, CriarRecompensaDto dto)
    {
        var recompensa = await _recompensaRepository.ObterPorIdAsync(id);

        if (recompensa == null)
        {
            return new RespostaMetodos<RetornoRecompensaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Recompensa não encontrada."
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(recompensa.FilhoId))
        {
            return new RespostaMetodos<RetornoRecompensaDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para editar esta recompensa"
            };
        }

        if (dto.FilhoId != recompensa.FilhoId && !await _autorizacao.PodeAcessarFilhoAsync(dto.FilhoId))
        {
            return new RespostaMetodos<RetornoRecompensaDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não pode mover esta recompensa para um filho que não é vinculado a você"
            };
        }

        recompensa.FilhoId = dto.FilhoId;
        recompensa.Descricao = dto.Descricao;
        recompensa.PontosNecessarios = dto.PontosNecessarios;

        await _recompensaRepository.AtualizarAsync(recompensa);
        var retornoRecompensa = recompensa.ToDto();

        return new RespostaMetodos<RetornoRecompensaDto>
        {
            Sucesso = true,
            ObjetoRetorno = retornoRecompensa,
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Recompensa atualizada com sucesso."
        };
    }

    public async Task<RespostaMetodos<RetornoRecompensaDto>> RemoverAsync(int id)
    {
        var recompensa = await _recompensaRepository.ObterPorIdAsync(id);

        if (recompensa == null)
        {
            return new RespostaMetodos<RetornoRecompensaDto>
            {
                Sucesso = false,
                Mensagem = "Recompensa não encontrada."
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(recompensa.FilhoId))
        {
            return new RespostaMetodos<RetornoRecompensaDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para remover esta recompensa"
            };
        }

        if (recompensa.Ativa)
        {
            recompensa.Ativa = false;
            await _recompensaRepository.AtualizarAsync(recompensa);

            return new RespostaMetodos<RetornoRecompensaDto>
            {
                Sucesso = true,
                StatusCode = HttpStatusCode.OK,
                Mensagem = "Recompensa desativada com sucesso."
            };
        }

        await _recompensaRepository.RemoverAsync(id);

        return new RespostaMetodos<RetornoRecompensaDto>
        {
            Sucesso = true,
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Recompensa removida com sucesso."
        };
    }

    public async Task<RespostaMetodos<RetornoRecompensaResgatadaDto>> ResgatarAsync(int filhoId, int recompensaId)
    {
        if (!await _autorizacao.PodeAcessarFilhoAsync(filhoId))
        {
            return new RespostaMetodos<RetornoRecompensaResgatadaDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para resgatar recompensas em nome deste filho"
            };
        }

        var recompensa = await _recompensaRepository.ObterPorIdAsync(recompensaId);

        if (recompensa == null)
        {
            return new RespostaMetodos<RetornoRecompensaResgatadaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Recompensa não encontrada."
            };
        }

        if (!recompensa.Ativa)
        {
            return new RespostaMetodos<RetornoRecompensaResgatadaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Recompensa não está ativa."
            };
        }

        var ganhos = await _pontuacaoRepository.ObterTotalPontosAsync(filhoId);
        var resgates = await _resgatePontuacaoRepository.ObterTotalResgatesAsync(filhoId);
        var saldoAtual = ganhos - resgates;

        if (saldoAtual < recompensa.PontosNecessarios)
        {
            return new RespostaMetodos<RetornoRecompensaResgatadaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Pontos insuficientes para resgatar esta recompensa."
            };
        }

        var resgatada = new RecompensaResgatada(filhoId, recompensaId);
        var resgate = ResgatePontuacao.Criar(filhoId, recompensaId, recompensa.PontosNecessarios);

        using (var transacao = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled))
        {
            await _recompensaRepository.ResgatarAsync(resgatada);
            await _resgatePontuacaoRepository.AdicionarAsync(resgate);

            transacao.Complete();
        }

        var retornoResgatada = resgatada.ToDto();

        return new RespostaMetodos<RetornoRecompensaResgatadaDto>
        {
            Sucesso = true,
            ObjetoRetorno = retornoResgatada,
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Recompensa resgatada com sucesso."
        };
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoRecompensaResgatadaDto>>> ObterResgatadasPorFilhoAsync(int filhoId)
    {
        if (!await _autorizacao.PodeAcessarFilhoAsync(filhoId))
        {
            return new RespostaMetodos<IEnumerable<RetornoRecompensaResgatadaDto>>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar as recompensas resgatadas deste filho"
            };
        }

        var resgatadas = await _recompensaRepository.ObterResgatadasPorFilhoAsync(filhoId);

        if (resgatadas == null || !resgatadas.Any())
        {
            return new RespostaMetodos<IEnumerable<RetornoRecompensaResgatadaDto>>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Nenhuma recompensa resgatada encontrada para este filho."
            };
        }

        var retornoResgatadas = resgatadas.ToDtoListResgatadas();

        return new RespostaMetodos<IEnumerable<RetornoRecompensaResgatadaDto>>
        {
            Sucesso = true,
            StatusCode = HttpStatusCode.OK,
            ObjetoRetorno = retornoResgatadas,
            Mensagem = "Recompensas resgatadas obtidas com sucesso."
        };
    }
}
