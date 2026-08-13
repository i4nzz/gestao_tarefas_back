using Microsoft.AspNetCore.Http;

namespace GestaoTarefas.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SalvarArquivoAsync(IFormFile arquivo, string subpasta);
    Task<(byte[] Conteudo, string ContentType)?> ObterArquivoAsync(string caminhoRelativo);
}
