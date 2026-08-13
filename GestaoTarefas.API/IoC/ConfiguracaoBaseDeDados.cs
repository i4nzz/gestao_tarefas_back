using GestaoTarefas.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.IoC;
/// <summary>
/// Configuração da base de dados para a aplicação, definindo o contexto do Entity Framework Core e a string de conexão para o banco de dados SQL Server. Essa configuração é essencial para garantir que a aplicação possa se conectar corretamente ao banco de dados e realizar operações de leitura e escrita de dados, permitindo a persistência das informações relacionadas aos usuários, tarefas, pontuações, recompensas e comprovações na aplicação de gestão de tarefas domésticas com gamificação.
/// </summary>
public static class ConfiguracaoBaseDeDados
{
    /// <summary>
    /// Adicionar o banco de dados.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContexto>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        return services;
    }
}
