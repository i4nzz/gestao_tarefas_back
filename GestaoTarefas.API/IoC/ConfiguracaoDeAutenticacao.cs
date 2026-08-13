using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GestaoTarefas.IoC;
/// <summary>
/// Configuração de autenticação JWT para a API, definindo os parâmetros de validação do token, como emissor, audiência, chave de assinatura e tempo de vida do token, além de configurar o esquema de autenticação e autorização para proteger os endpoints da API.
/// </summary>
public static class ConfiguracaoDeAutenticacao
{
    /// <summary>
    /// Adiciona a configuração de autenticação JWT ao serviço de injeção de dependência, definindo os parâmetros de validação do token e configurando o esquema de autenticação e autorização para a API.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        services.AddAuthorization();

        return services;
    }
}
