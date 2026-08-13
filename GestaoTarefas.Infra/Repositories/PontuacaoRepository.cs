using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Infra.Repositories;

public class PontuacaoRepository : IPontuacaoRepository
{
    private readonly AppDbContexto _contexto;

    public PontuacaoRepository(AppDbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Pontuacao>> ObterPorFilhoAsync(int filhoId)
    {
        return await _contexto.Pontuacoes
            .Include(p => p.Tarefa)
            .Where(p => p.FilhoId == filhoId)
            .OrderByDescending(p => p.DataRegistro)
            .ToListAsync();
    }

    public async Task<int> ObterTotalPontosAsync(int filhoId)
    {
        return await _contexto.Pontuacoes
            .Where(p => p.FilhoId == filhoId)
            .SumAsync(p => p.Pontos);
    }

    public async Task AdicionarAsync(Pontuacao pontuacao)
    {
        await _contexto.Pontuacoes.AddAsync(pontuacao);
        await _contexto.SaveChangesAsync();
    }

    public async Task<bool> ExisteAsync(int tarefaId, int filhoId)
    {
        return await _contexto.Pontuacoes.AnyAsync(p => p.TarefaId == tarefaId && p.FilhoId == filhoId);
    }
}
