using GestaoTarefas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Infra.Data;

public class AppDbContexto : DbContext
{
    public AppDbContexto(DbContextOptions<AppDbContexto> options) : base(options) { }

    #region Usuários
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Pai> Pais { get; set; }
    public DbSet<Filho> Filhos { get; set; }
    public DbSet<PaisFilhos> PaisFilhos { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    #endregion

    #region Gameficação
    public DbSet<Tarefa> Tarefas { get; set; }
    public DbSet<ComprovacaoTarefa> ComprovacoesTarefa { get; set; }
    public DbSet<Pontuacao> Pontuacoes { get; set; }
    public DbSet<Recompensa> Recompensas { get; set; }
    public DbSet<RecompensaResgatada> RecompensasResgatadas { get; set; }

    #endregion
    public DbSet<Mesada> Mesadas { get; set; }
    public DbSet<RegistroFinanceiro> RegistrosFinanceiros { get; set; }
    public DbSet<CategoriaFinanceira> CategoriasFinanceiras { get; set; }
    public DbSet<ResgatePontuacao> ResgatesPontuacao { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContexto).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}