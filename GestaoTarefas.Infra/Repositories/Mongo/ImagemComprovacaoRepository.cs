using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data.Mongo;
using MongoDB.Driver;

namespace GestaoTarefas.Infra.Repositories.Mongo;

/// <summary>
/// Implementação do repositório de imagens de comprovação usando MongoDB. As imagens são salvas
/// diretamente como bytes na coleção "imagensAplicacao", junto com a data/hora de salvamento.
/// </summary>
public class ImagemComprovacaoRepository : IImagemComprovacaoRepository
{
    private readonly IMongoCollection<ImagemAplicacaoDocumento> _colecao;

    public ImagemComprovacaoRepository(IMongoDatabase database, MongoDbSettings settings)
    {
        _colecao = database.GetCollection<ImagemAplicacaoDocumento>(settings.ImagensCollectionName);
    }

    public async Task<string> SalvarAsync(byte[] conteudo, string contentType, string nomeArquivoOriginal)
    {
        var documento = new ImagemAplicacaoDocumento
        {
            Conteudo = conteudo,
            ContentType = contentType,
            NomeArquivoOriginal = nomeArquivoOriginal,
            DataSalvamento = DateTime.UtcNow
        };

        await _colecao.InsertOneAsync(documento);

        return documento.Id;
    }

    public async Task<ImagemComprovacao?> ObterPorIdAsync(string id)
    {
        var documento = await _colecao.Find(d => d.Id == id).FirstOrDefaultAsync();

        if (documento == null)
        {
            return null;
        }

        return new ImagemComprovacao(documento.Id, documento.Conteudo, documento.ContentType, documento.NomeArquivoOriginal, documento.DataSalvamento);
    }

    public async Task AtualizarAsync(string id, byte[] conteudo, string contentType, string nomeArquivoOriginal)
    {
        var atualizacao = Builders<ImagemAplicacaoDocumento>.Update
            .Set(d => d.Conteudo, conteudo)
            .Set(d => d.ContentType, contentType)
            .Set(d => d.NomeArquivoOriginal, nomeArquivoOriginal)
            .Set(d => d.DataSalvamento, DateTime.UtcNow);

        await _colecao.UpdateOneAsync(d => d.Id == id, atualizacao);
    }

    public async Task RemoverAsync(string id)
    {
        await _colecao.DeleteOneAsync(d => d.Id == id);
    }
}
