using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data;
using Microsoft.EntityFrameworkCore;


namespace GestaoTarefas.Infra.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContexto _context;

    public UsuarioRepository(AppDbContexto context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorTokenResetSenhaAsync(string token)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenResetSenha == token);
    }

    public async Task<Usuario?> ObterPorIdAsync(int id)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AdicionarFilhoAsync(Filho filho, PaisFilhos vinculo)
    {
        await _context.Filhos.AddAsync(filho);
        await _context.SaveChangesAsync(); // salva primeiro para gerar o Id

        var paisFilhos = new PaisFilhos(vinculo.PaiId, filho.Id);
        await _context.PaisFilhos.AddAsync(paisFilhos);
        await _context.SaveChangesAsync();
    }
    public async Task<Usuario?> ObterPorTokenConfirmacaoEmailAsync(string token)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenConfirmacaoEmail == token);
    }

    public async Task<IEnumerable<Usuario>> ObterTodosAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<bool> ExisteVinculoAsync(int paiId, int filhoId)
    {
        return await _context.PaisFilhos.AnyAsync(x => x.PaiId == paiId && x.FilhoId == filhoId);
    }

    public async Task<bool> PossuiVinculoFamiliarAsync(int usuarioId)
    {
        return await _context.PaisFilhos.AnyAsync(x => x.PaiId == usuarioId || x.FilhoId == usuarioId);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }
}