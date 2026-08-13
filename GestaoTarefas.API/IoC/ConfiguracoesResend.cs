using GestaoTarefas.Application.Configuration;
using Resend;

namespace GestaoTarefas.IoC;

/// <summary>
/// Configuracoes para integração com o serviço de envio de e-mails Resend, incluindo a configuração da API Key e a injeção de dependência do cliente ResendClient para utilização nos serviços de envio de e-mails da aplicação.
/// </summary>
public static class ConfiguracoesResend
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddResendEmail(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Mapeia a seção "Resend" do appsettings para a classe ResendOptions
        services.Configure<ResendOptions>(configuration.GetSection(ResendOptions.SectionName));

        // 2. HttpClient necessário internamente pelo ResendClient
        services.AddHttpClient<ResendClient>();

        // 3. Configura o ResendClientOptions com a API Key lida do appsettings
        services.Configure<ResendClientOptions>(options =>
        {
            options.ApiToken = configuration.GetSection(ResendOptions.SectionName)[nameof(ResendOptions.ApiKey)]
                ?? throw new InvalidOperationException("API Key do Resend não configurada.");
        });

        // 4. Registra o cliente do Resend
        services.AddTransient<IResend, ResendClient>();

        return services;
    }
}
