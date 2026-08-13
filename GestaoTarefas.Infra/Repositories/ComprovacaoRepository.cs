using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Infra.Repositories;

public class ComprovacaoRepository : IComprovacaoRepository
{
    private readonly AppDbContexto _contexto;

    public ComprovacaoRepository(AppDbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<ComprovacaoTarefa>> ObterPorTarefaAsync(int tarefaId)
    {
        return await _contexto.ComprovacoesTarefa
            .Include(c => c.Tarefa)
            .Where(c => c.TarefaId == tarefaId)
            .OrderByDescending(c => c.DataEnvio)
            .ToListAsync();
    }

    public async Task<ComprovacaoTarefa?> ObterPorIdAsync(int id)
    {
        return await _contexto.ComprovacoesTarefa
            .Include(c => c.Tarefa)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AdicionarAsync(ComprovacaoTarefa comprovacao)
    {
        await _contexto.ComprovacoesTarefa.AddAsync(comprovacao);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(ComprovacaoTarefa comprovacao)
    {
        _contexto.ComprovacoesTarefa.Update(comprovacao);
        await _contexto.SaveChangesAsync();
    }
}
