using System.Net;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.RegistroFinanceiro;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Mapping;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;

namespace GestaoTarefas.Application.Services;

public class RegistroFinanceiroService : IRegistroFinanceiroService
{
    private readonly IRegistroFinanceiroRepository _registroRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICategoriaFinanceiraRepository _categoriaRepository;
    private readonly IMesadaRepository _mesadaRepository;
    private readonly IAutorizacaoFamiliarService _autorizacao;

    public RegistroFinanceiroService(
        IRegistroFinanceiroRepository registroRepository
        , IUsuarioRepository usuarioRepository
        , ICategoriaFinanceiraRepository categoriaRepository
        , IMesadaRepository mesadaRepository
        , IAutorizacaoFamiliarService autorizacao
        )
    {
        _registroRepository = registroRepository;
        _usuarioRepository = usuarioRepository;
        _categoriaRepository = categoriaRepository;
        _mesadaRepository = mesadaRepository;
        _autorizacao = autorizacao;
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoRegistroFinanceiroDto>>> ObterPorFilhoAsync(int filhoId)
    {
        if (!await _autorizacao.PodeAcessarFilhoAsync(filhoId))
        {
            return new RespostaMetodos<IEnumerable<RetornoRegistroFinanceiroDto>>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar os registros financeiros deste filho"
            };
        }

        var registros = await _registroRepository.ObterPorFilhoAsync(filhoId);

        return new RespostaMetodos<IEnumerable<RetornoRegistroFinanceiroDto>>
        {
            Sucesso = true,
            ObjetoRetorno = registros.ToDtoList(),
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Registros financeiros obtidos com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoRegistroFinanceiroDto>> CriarAsync(CriarRegistroFinanceiroDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(dto.FilhoId);

        if (usuario == null)
        {
            return new RespostaMetodos<RetornoRegistroFinanceiroDto>
            {
                Sucesso = false,
                Mensagem = "Filho não encontrado"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(dto.FilhoId))
        {
            return new RespostaMetodos<RetornoRegistroFinanceiroDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não pode registrar gastos para um filho que não é vinculado a você"
            };
        }

        if (dto.Valor <= 0)
        {
            return new RespostaMetodos<RetornoRegistroFinanceiroDto>
            {
                Sucesso = false,
                Mensagem = "O valor do registro deve ser maior que zero"
            };
        }

        var categoria = await _categoriaRepository.ObterPorIdAsync(dto.CategoriaId);

        if (categoria == null)
        {
            return new RespostaMetodos<RetornoRegistroFinanceiroDto>
            {
                Sucesso = false,
                Mensagem = "Categoria financeira não encontrada"
            };
        }

        var mesada = await _mesadaRepository.ObterPorIdAsync(dto.MesadaId);

        if (mesada == null)
        {
            return new RespostaMetodos<RetornoRegistroFinanceiroDto>
            {
                Sucesso = false,
                Mensagem = "Mesada não encontrada"
            };
        }

        if (mesada.FilhoId != dto.FilhoId)
        {
            return new RespostaMetodos<RetornoRegistroFinanceiroDto>
            {
                Sucesso = false,
                Mensagem = "A mesada informada não pertence ao filho informado"
            };
        }

        var totalJaGasto = await _registroRepository.ObterTotalGastoPorMesadaAsync(dto.MesadaId);
        var saldoDisponivel = mesada.Valor - totalJaGasto;

        if (dto.Valor > saldoDisponivel)
        {
            return new RespostaMetodos<RetornoRegistroFinanceiroDto>
            {
                Sucesso = false,
                Mensagem = $"Saldo insuficiente na mesada. Saldo disponível: {saldoDisponivel:C}"
            };
        }

        var registro = new RegistroFinanceiro
        {
            FilhoId = dto.FilhoId,
            CategoriaId = dto.CategoriaId,
            MesadaId = dto.MesadaId,
            Descricao = dto.Descricao,
            Valor = dto.Valor
        };

        await _registroRepository.AdicionarAsync(registro);
        registro.Categoria = categoria;

        return new RespostaMetodos<RetornoRegistroFinanceiroDto>
        {
            Sucesso = true,
            ObjetoRetorno = registro.ToDto(),
            StatusCode = HttpStatusCode.Created,
            Mensagem = "Registro financeiro criado com sucesso"
        };
    }

    public async Task<RespostaMetodos<ResumoFinanceiroFilhoDto>> ObterResumoPorFilhoAsync(int filhoId)
    {
        if (!await _autorizacao.PodeAcessarFilhoAsync(filhoId))
        {
            return new RespostaMetodos<ResumoFinanceiroFilhoDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar o resumo financeiro deste filho"
            };
        }

        var usuario = await _usuarioRepository.ObterPorIdAsync(filhoId);

        if (usuario == null)
        {
            return new RespostaMetodos<ResumoFinanceiroFilhoDto>
            {
                Sucesso = false,
                Mensagem = "Filho não encontrado"
            };
        }

        var mesadas = await _mesadaRepository.ObterPorFilhoAsync(filhoId);
        var registros = await _registroRepository.ObterPorFilhoAsync(filhoId);

        var totalMesadas = mesadas.Sum(m => m.Valor);
        var totalGasto = registros.Sum(r => r.Valor);

        var gastosPorCategoria = registros
            .GroupBy(r => new { r.CategoriaId, NomeCategoria = r.Categoria?.Nome ?? string.Empty })
            .Select(g => new GastoPorCategoriaDto
            {
                CategoriaId = g.Key.CategoriaId,
                NomeCategoria = g.Key.NomeCategoria,
                Total = g.Sum(r => r.Valor),
                Percentual = totalGasto == 0 ? 0 : (double)(g.Sum(r => r.Valor) / totalGasto) * 100
            })
            .OrderByDescending(g => g.Total)
            .ToList();

        var resumo = new ResumoFinanceiroFilhoDto
        {
            FilhoId = filhoId,
            NomeFilho = usuario.Nome,
            TotalMesadas = totalMesadas,
            TotalGasto = totalGasto,
            SaldoDisponivel = totalMesadas - totalGasto,
            GastosPorCategoria = gastosPorCategoria
        };

        return new RespostaMetodos<ResumoFinanceiroFilhoDto>
        {
            Sucesso = true,
            ObjetoRetorno = resumo,
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Resumo financeiro obtido com sucesso"
        };
    }
}
