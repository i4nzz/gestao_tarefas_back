using System.Net;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Mesada;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Mapping;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;

namespace GestaoTarefas.Application.Services;

public class MesadaService : IMesadaService
{
    private readonly IMesadaRepository _mesadaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRegistroFinanceiroRepository _registroRepository;
    private readonly IAutorizacaoFamiliarService _autorizacao;

    public MesadaService(
        IMesadaRepository mesadaRepository
        , IUsuarioRepository usuarioRepository
        , IRegistroFinanceiroRepository registroRepository
        , IAutorizacaoFamiliarService autorizacao
        )
    {
        _mesadaRepository = mesadaRepository;
        _usuarioRepository = usuarioRepository;
        _registroRepository = registroRepository;
        _autorizacao = autorizacao;
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoMesadaDto>>> ObterPorFilhoAsync(int filhoId)
    {
        if (!await _autorizacao.PodeAcessarFilhoAsync(filhoId))
        {
            return new RespostaMetodos<IEnumerable<RetornoMesadaDto>>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar as mesadas deste filho"
            };
        }

        var mesadas = await _mesadaRepository.ObterPorFilhoAsync(filhoId);

        var retornoMesadas = new List<RetornoMesadaDto>();

        foreach (var mesada in mesadas)
        {
            var valorGasto = await _registroRepository.ObterTotalGastoPorMesadaAsync(mesada.MesadaId);
            retornoMesadas.Add(mesada.ToDto(valorGasto));
        }

        return new RespostaMetodos<IEnumerable<RetornoMesadaDto>>
        {
            Sucesso = true,
            ObjetoRetorno = retornoMesadas,
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Mesadas obtidas com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoMesadaDto>> CriarAsync(CriarMesadaDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(dto.FilhoId);

        if (usuario == null)
        {
            return new RespostaMetodos<RetornoMesadaDto>
            {
                Sucesso = false,
                Mensagem = "Filho não encontrado"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(dto.FilhoId))
        {
            return new RespostaMetodos<RetornoMesadaDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não pode registrar mesada para um filho que não é vinculado a você"
            };
        }

        if (dto.Valor <= 0)
        {
            return new RespostaMetodos<RetornoMesadaDto>
            {
                Sucesso = false,
                Mensagem = "O valor da mesada deve ser maior que zero"
            };
        }

        if (dto.Mes is < 1 or > 12)
        {
            return new RespostaMetodos<RetornoMesadaDto>
            {
                Sucesso = false,
                Mensagem = "O mês deve estar entre 1 e 12"
            };
        }

        var mesada = new Mesada
        {
            FilhoId = dto.FilhoId,
            Valor = dto.Valor,
            Mes = dto.Mes,
            Ano = dto.Ano
        };

        await _mesadaRepository.AdicionarAsync(mesada);

        return new RespostaMetodos<RetornoMesadaDto>
        {
            Sucesso = true,
            ObjetoRetorno = mesada.ToDto(),
            StatusCode = HttpStatusCode.Created,
            Mensagem = "Mesada registrada com sucesso"
        };
    }
}
