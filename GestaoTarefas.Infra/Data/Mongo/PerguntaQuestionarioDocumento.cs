using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GestaoTarefas.Infra.Data.Mongo;

/// <summary>
/// Documento persistido na coleção "bancoQuestionario" do MongoDB. Os campos seguem o formato
/// camelCase usado nos documentos da coleção.
/// </summary>
[BsonIgnoreExtraElements]
public class PerguntaQuestionarioDocumento
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("categoria")]
    public string Categoria { get; set; } = string.Empty;

    [BsonElement("dificuldade")]
    public string Dificuldade { get; set; } = string.Empty;

    [BsonElement("pergunta")]
    public string Pergunta { get; set; } = string.Empty;

    [BsonElement("opcoes")]
    public List<string> Opcoes { get; set; } = new();

    [BsonElement("respostaCorreta")]
    public int RespostaCorreta { get; set; }

    [BsonElement("explicacao")]
    public string Explicacao { get; set; } = string.Empty;

    [BsonElement("pontos")]
    public int Pontos { get; set; }

    [BsonElement("ativa")]
    public bool Ativa { get; set; } = true;
}
