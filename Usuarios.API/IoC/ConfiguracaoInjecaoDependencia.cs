using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Services;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Repositories;

namespace GestaoTarefas.Ioc;

/// <summary>
/// Classe para configurar a inje��o de depend�ncia dos servi�os e reposit�rios da aplica��o, registrando as implementa��es concretas para as interfaces correspondentes. Essa configura��o � essencial para garantir que as depend�ncias sejam resolvidas corretamente em tempo de execu��o, permitindo a invers�o de controle e facilitando a manuten��o e testabilidade do c�digo.
/// </summary>
public static class ConfiguracaoInjecaoDeDependencia
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        #region Services
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<ITarefaService, TarefaService>();
        services.AddScoped<IPontuacaoService, PontuacaoService>();
        services.AddScoped<IRecompensaService, RecompensaService>();
        services.AddScoped<IComprovacaoService, ComprovacaoService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAutorizacaoFamiliarService, AutorizacaoFamiliarService>();
        #endregion

        #region Repositories
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ITarefaRepository, TarefaRepository>();
        services.AddScoped<IPontuacaoRepository, PontuacaoRepository>();
        services.AddScoped<IRecompensaRepository, RecompensaRepository>();
        services.AddScoped<IComprovacaoRepository, ComprovacaoRepository>();
        services.AddScoped<IResgatePontuacaoRepository, ResgatePontuacaoRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        #endregion

        return services;
    }
}