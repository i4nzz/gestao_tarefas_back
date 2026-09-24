using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GestaoTarefas.Infra.Data.Mongo;

/// <summary>
/// Documento persistido na coleção "imagensAplicacao" do MongoDB, contendo os bytes da imagem
/// e a data/hora em que foi salva.
/// </summary>
public class ImagemAplicacaoDocumento
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public byte[] Conteudo { get; set; } = Array.Empty<byte>();

    public string ContentType { get; set; } = string.Empty;

    public string NomeArquivoOriginal { get; set; } = string.Empty;

    public DateTime DataSalvamento { get; set; }
}
