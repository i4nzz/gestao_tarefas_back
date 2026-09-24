using GestaoTarefas.Infra.Data.Mongo;
using MongoDB.Driver;

namespace GestaoTarefas.IoC;

/// <summary>
/// Configuração da conexão com o MongoDB, usado para armazenar as imagens de comprovação de tarefa
/// (bytes + data/hora de salvamento) na coleção "imagensAplicacao".
/// </summary>
public static class ConfiguracaoMongoDb
{
    /// <summary>
    /// Adicionar a configuração de conexão com o MongoDB.
    /// </summary>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>()
            ?? throw new InvalidOperationException("Seção 'MongoDbSettings' não configurada em appsettings.json");

        services.AddSingleton(settings);

        services.AddSingleton<IMongoClient>(new MongoClient(settings.ConnectionString));

        services.AddSingleton(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase(settings.DatabaseName));

        return services;
    }
}
