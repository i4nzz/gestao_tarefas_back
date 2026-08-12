using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Infra.Repositories;

public class RegistroFinanceiroRepository : IRegistroFinanceiroRepository
{
    private readonly AppDbContexto _contexto;

    public RegistroFinanceiroRepository(AppDbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<RegistroFinanceiro>> ObterPorFilhoAsync(int filhoId)
    {
        return await _contexto.RegistrosFinanceiros
            .Include(r => r.Filho)
            .Include(r => r.Categoria)
            .Where(r => r.FilhoId == filhoId)
            .OrderByDescending(r => r.DataRegistro)
            .ToListAsync();
    }

    public async Task AdicionarAsync(RegistroFinanceiro registro)
    {
        await _contexto.RegistrosFinanceiros.AddAsync(registro);
        await _contexto.SaveChangesAsync();
    }
}
