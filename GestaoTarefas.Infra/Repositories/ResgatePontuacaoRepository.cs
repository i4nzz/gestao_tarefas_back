using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Infra.Repositories;

public class ResgatePontuacaoRepository : IResgatePontuacaoRepository
{
    private readonly AppDbContexto _contexto;

    public ResgatePontuacaoRepository(AppDbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task AdicionarAsync(ResgatePontuacao resgate)
    {
        await _contexto.ResgatesPontuacao.AddAsync(resgate);
        await _contexto.SaveChangesAsync();
    }

    public async Task<IEnumerable<ResgatePontuacao>> ObterPorFilhoAsync(int filhoId)
    {
        return await _contexto.ResgatesPontuacao
            .Include(r => r.Recompensa)
            .Where(r => r.FilhoId == filhoId)
            .OrderByDescending(r => r.DataResgate)
            .ToListAsync();
    }

    public async Task<int> ObterTotalResgatesAsync(int filhoId)
    {
        return await _contexto.ResgatesPontuacao
            .Where(r => r.FilhoId == filhoId)
            .SumAsync(r => r.Pontos);
    }
}
