using Microsoft.EntityFrameworkCore;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data;

namespace GestaoTarefas.Infra.Repositories;

public class TarefaRepository : ITarefaRepository
{
    private readonly AppDbContexto _context;

    public TarefaRepository(AppDbContexto context)
    {
        _context = context;
    }
    public async Task AdicionarAsync(Tarefa tarefa)
    {
        await _context.Tarefas.AddAsync(tarefa);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Tarefa tarefa)
    {
        _context.Tarefas.Update(tarefa);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Tarefa>> ObterPorFilhoAsync(int filhoId)
    {
        return await _context.Tarefas
            .Include(t => t.Comprovacoes)
            .Where(t => t.FilhoId == filhoId)
            .ToListAsync();
    }

    public async Task<Tarefa?> ObterPorIdAsync(int tarefaId)
    {
        return await _context.Tarefas
            .Include(t => t.Filho)
            .Include(t => t.Comprovacoes)
            .FirstOrDefaultAsync(t => t.TarefaId == tarefaId);
    }

    public async Task<IEnumerable<Tarefa>> ObterTodasAsync()
    {
        return await _context.Tarefas
           .Include(t => t.Filho)
           .ToListAsync();
    }

    public async Task RemoverAsync(int tarefaId)
    {
        var tarefa = await ObterPorIdAsync(tarefaId);

        if (tarefa != null)
        {
            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();
        }
    }
}
