using brevo_csharp.Api;
using brevo_csharp.Client;
using GestaoTarefas.Application.Configuration;
using Microsoft.Extensions.Options;

namespace GestaoTarefas.IoC;

/// <summary>
/// Configuracoes para integração com o serviço de envio de e-mails Brevo, incluindo a configuração da API Key e a injeção de dependência do cliente TransactionalEmailsApi para utilização nos serviços de envio de e-mails da aplicação.
/// </summary>
public static class ConfiguracoesBrevo
{
    /// <summary>
    /// Registra o cliente da API transacional do Brevo e suas configurações na coleção de serviços.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddBrevoEmail(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Mapeia a seção "Brevo" do appsettings para a classe BrevoOptions
        services.Configure<BrevoOptions>(configuration.GetSection(BrevoOptions.SectionName));

        // 2. Registra o cliente do Brevo com a API Key lida do appsettings
        services.AddTransient<ITransactionalEmailsApi>(sp =>
        {
            var brevoOptions = sp.GetRequiredService<IOptions<BrevoOptions>>().Value;

            if (string.IsNullOrWhiteSpace(brevoOptions.ApiKey))
            {
                throw new InvalidOperationException("API Key do Brevo não configurada.");
            }

            var brevoConfiguration = new Configuration();
            brevoConfiguration.ApiKey.Add("api-key", brevoOptions.ApiKey);

            return new TransactionalEmailsApi(brevoConfiguration);
        });

        return services;
    }
}
