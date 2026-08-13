using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Infra.Repositories;

public class RecompensaRepository : IRecompensaRepository
{
    private readonly AppDbContexto _contexto;

    public RecompensaRepository(AppDbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Recompensa>> ObterPorFilhoAsync(int filhoId)
    {
        return await _contexto.Recompensas
            .Include(r => r.Filho)
            .Where(r => r.FilhoId == filhoId && r.Ativa)
            .ToListAsync();
    }

    public async Task<Recompensa?> ObterPorIdAsync(int id)
    {
        return await _contexto.Recompensas
            .Include(r => r.Filho)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AdicionarAsync(Recompensa recompensa)
    {
        await _contexto.Recompensas.AddAsync(recompensa);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Recompensa recompensa)
    {
        _contexto.Recompensas.Update(recompensa);
        await _contexto.SaveChangesAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var recompensa = await ObterPorIdAsync(id);
        if (recompensa != null)
        {
            _contexto.Recompensas.Remove(recompensa);
            await _contexto.SaveChangesAsync();
        }
    }

    public async Task ResgatarAsync(RecompensaResgatada recompensaResgatada)
    {
        await _contexto.RecompensasResgatadas.AddAsync(recompensaResgatada);
        await _contexto.SaveChangesAsync();
    }

    public async Task<IEnumerable<RecompensaResgatada>> ObterResgatadasPorFilhoAsync(int filhoId)
    {
        return await _contexto.RecompensasResgatadas
            .Include(r => r.Recompensa)
            .Include(r => r.Filho)
            .Where(r => r.FilhoId == filhoId)
            .OrderByDescending(r => r.DataResgate)
            .ToListAsync();
    }
}
