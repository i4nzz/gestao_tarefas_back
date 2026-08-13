using GestaoTarefas.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GestaoTarefas.Application.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private static readonly string[] ExtensoesPermitidas = { ".jpg", ".jpeg", ".png" };
    private const long TamanhoMaximoBytes = 5 * 1024 * 1024; // 5MB

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        // fora do wwwroot de propósito — StaticFiles nunca serve essa pasta
        _basePath = Path.Combine(env.ContentRootPath, "StorageFiles");
    }

    public async Task<string> SalvarArquivoAsync(IFormFile arquivo, string subpasta)
    {
        if (arquivo == null || arquivo.Length == 0)
        {
            throw new ArgumentException("Arquivo inválido ou vazio.");
        }

        if (arquivo.Length > TamanhoMaximoBytes)
        {
            throw new ArgumentException("Arquivo excede o tamanho máximo permitido (5MB).");
        }

        var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();

        if (!ExtensoesPermitidas.Contains(extensao))
        {
            throw new ArgumentException("Tipo de arquivo não permitido. Use JPG ou PNG.");
        }

        var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
        var pastaCompleta = Path.Combine(_basePath, subpasta);

        Directory.CreateDirectory(pastaCompleta);

        var caminhoCompleto = Path.Combine(pastaCompleta, nomeArquivo);

        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
        {
            await arquivo.CopyToAsync(stream);
        }

        return Path.Combine(subpasta, nomeArquivo).Replace("\\", "/");
    }

    public async Task<(byte[] Conteudo, string ContentType)?> ObterArquivoAsync(string caminhoRelativo)
    {
        var caminhoCompleto = Path.Combine(_basePath, caminhoRelativo);

        if (!File.Exists(caminhoCompleto))
        {
            return null;
        }

        var bytes = await File.ReadAllBytesAsync(caminhoCompleto);
        var contentType = ObterContentType(caminhoRelativo);

        return (bytes, contentType);
    }

    private static string ObterContentType(string caminho)
    {
        var extensao = Path.GetExtension(caminho).ToLowerInvariant();

        return extensao switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}
