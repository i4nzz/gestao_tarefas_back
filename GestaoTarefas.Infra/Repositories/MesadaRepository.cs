using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Infra.Repositories;

public class MesadaRepository : IMesadaRepository
{
    private readonly AppDbContexto _contexto;

    public MesadaRepository(AppDbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Mesada>> ObterPorFilhoAsync(int filhoId)
    {
        return await _contexto.Mesadas
            .Include(m => m.Filho)
            .Where(m => m.FilhoId == filhoId && m.Ativa)
            .OrderByDescending(m => m.Ano)
            .ThenByDescending(m => m.Mes)
            .ToListAsync();
    }

    public async Task<Mesada?> ObterPorIdAsync(int id)
    {
        return await _contexto.Mesadas
            .Include(m => m.Filho)
            .FirstOrDefaultAsync(m => m.MesadaId == id);
    }

    public async Task AdicionarAsync(Mesada mesada)
    {
        await _contexto.Mesadas.AddAsync(mesada);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Mesada mesada)
    {
        _contexto.Mesadas.Update(mesada);
        await _contexto.SaveChangesAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var mesada = await ObterPorIdAsync(id);
        if (mesada != null)
        {
            _contexto.Mesadas.Remove(mesada);
            await _contexto.SaveChangesAsync();
        }
    }
}
