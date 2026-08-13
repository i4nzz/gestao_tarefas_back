using System.Net;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.CategoriaFinanceira;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Mapping;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;

namespace GestaoTarefas.Application.Services;

public class CategoriaFinanceiraService : ICategoriaFinanceiraService
{
    private readonly ICategoriaFinanceiraRepository _categoriaRepository;

    public CategoriaFinanceiraService(ICategoriaFinanceiraRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoCategoriaFinanceiraDto>>> ObterTodasAsync()
    {
        var categorias = await _categoriaRepository.ObterTodasAsync();

        if (categorias == null || !categorias.Any())
        {
            return new RespostaMetodos<IEnumerable<RetornoCategoriaFinanceiraDto>>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Nenhuma categoria financeira encontrada"
            };
        }

        return new RespostaMetodos<IEnumerable<RetornoCategoriaFinanceiraDto>>
        {
            Sucesso = true,
            ObjetoRetorno = categorias.ToDtoList(),
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Categorias financeiras obtidas com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoCategoriaFinanceiraDto?>> ObterPorIdAsync(int id)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(id);

        if (categoria == null)
        {
            return new RespostaMetodos<RetornoCategoriaFinanceiraDto?>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Categoria financeira não encontrada"
            };
        }

        return new RespostaMetodos<RetornoCategoriaFinanceiraDto?>
        {
            Sucesso = true,
            ObjetoRetorno = categoria.ToDto(),
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Categoria financeira obtida com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoCategoriaFinanceiraDto>> CriarAsync(CriarCategoriaFinanceiraDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            return new RespostaMetodos<RetornoCategoriaFinanceiraDto>
            {
                Sucesso = false,
                Mensagem = "O nome da categoria é obrigatório"
            };
        }

        var categoria = new CategoriaFinanceira { Nome = dto.Nome.Trim() };

        await _categoriaRepository.AdicionarAsync(categoria);

        return new RespostaMetodos<RetornoCategoriaFinanceiraDto>
        {
            Sucesso = true,
            ObjetoRetorno = categoria.ToDto(),
            StatusCode = HttpStatusCode.Created,
            Mensagem = "Categoria financeira criada com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoCategoriaFinanceiraDto>> AtualizarAsync(int id, CriarCategoriaFinanceiraDto dto)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(id);

        if (categoria == null)
        {
            return new RespostaMetodos<RetornoCategoriaFinanceiraDto>
            {
                Sucesso = false,
                Mensagem = "Categoria financeira não encontrada"
            };
        }

        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            return new RespostaMetodos<RetornoCategoriaFinanceiraDto>
            {
                Sucesso = false,
                Mensagem = "O nome da categoria é obrigatório"
            };
        }

        categoria.Nome = dto.Nome.Trim();

        await _categoriaRepository.AtualizarAsync(categoria);

        return new RespostaMetodos<RetornoCategoriaFinanceiraDto>
        {
            Sucesso = true,
            ObjetoRetorno = categoria.ToDto(),
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Categoria financeira atualizada com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoCategoriaFinanceiraDto>> RemoverAsync(int id)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(id);

        if (categoria == null)
        {
            return new RespostaMetodos<RetornoCategoriaFinanceiraDto>
            {
                Sucesso = false,
                Mensagem = "Categoria financeira não encontrada"
            };
        }

        await _categoriaRepository.RemoverAsync(id);

        return new RespostaMetodos<RetornoCategoriaFinanceiraDto>
        {
            Sucesso = true,
            StatusCode = HttpStatusCode.OK,
            Mensagem = "Categoria financeira removida com sucesso"
        };
    }
}
