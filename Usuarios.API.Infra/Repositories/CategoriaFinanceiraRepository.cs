using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Infra.Repositories;

public class CategoriaFinanceiraRepository : ICategoriaFinanceiraRepository
{
    private readonly AppDbContexto _contexto;

    public CategoriaFinanceiraRepository(AppDbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<CategoriaFinanceira>> ObterTodasAsync()
    {
        return await _contexto.CategoriasFinanceiras
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<CategoriaFinanceira?> ObterPorIdAsync(int id)
    {
        return await _contexto.CategoriasFinanceiras
            .FirstOrDefaultAsync(c => c.CategoriaFinanceiraId == id);
    }

    public async Task AdicionarAsync(CategoriaFinanceira categoria)
    {
        await _contexto.CategoriasFinanceiras.AddAsync(categoria);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(CategoriaFinanceira categoria)
    {
        _contexto.CategoriasFinanceiras.Update(categoria);
        await _contexto.SaveChangesAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var categoria = await ObterPorIdAsync(id);
        if (categoria != null)
        {
            _contexto.CategoriasFinanceiras.Remove(categoria);
            await _contexto.SaveChangesAsync();
        }
    }
}
