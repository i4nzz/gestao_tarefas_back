namespace GestaoTarefas.Infra.Data.Mongo;

/// <summary>
/// Configurações de conexão com o MongoDB, lidas da seção "MongoDbSettings" do appsettings.json.
/// </summary>
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ImagensCollectionName { get; set; } = "imagensAplicacao";
}
