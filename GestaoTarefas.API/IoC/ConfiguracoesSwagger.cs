using System.Reflection;
using Microsoft.OpenApi.Models;

namespace GestaoTarefas.IoC;
/// <summary>
/// Provides extension methods for configuring Swagger in the API application.
/// </summary>
/// <remarks>Includes methods to add Swagger services and middleware, configure API documentation, and set up JWT
/// Bearer authentication for Swagger UI.</remarks>
public static class ConfiguracoesSwagger
{
    /// <summary>
    /// Configures Swagger generation and JWT Bearer authentication for the API documentation.
    /// </summary>
    /// <param name="services">The service collection to add the Swagger configuration to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API.GestaoTarefas",
                Version = "v1",
                Description = "Sistema de tarefas domésticas com gamificação."
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT assim: Bearer eyjwt123..."
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
    /// <summary>
    /// Usar as configurações de Swagger e configurar a interface do usuário do Swagger para a API, definindo o título do documento e o endpoint do Swagger JSON.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "API.GestaoTarefas";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API.GestaoTarefas v1");
        });

        return app;
    }
}
