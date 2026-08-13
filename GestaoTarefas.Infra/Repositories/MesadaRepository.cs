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
            .Where(m => m.FilhoId == filhoId)
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
}
