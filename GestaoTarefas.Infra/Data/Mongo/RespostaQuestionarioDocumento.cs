using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GestaoTarefas.Infra.Data.Mongo;

/// <summary>
/// Documento persistido na coleção "respostasQuestionario" do MongoDB: uma resposta de um filho.
/// </summary>
[BsonIgnoreExtraElements]
public class RespostaQuestionarioDocumento
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("filhoId")]
    public int FilhoId { get; set; }

    [BsonElement("perguntaId")]
    public string PerguntaId { get; set; } = string.Empty;

    [BsonElement("categoria")]
    public string Categoria { get; set; } = string.Empty;

    [BsonElement("dificuldade")]
    public string Dificuldade { get; set; } = string.Empty;

    [BsonElement("pergunta")]
    public string Pergunta { get; set; } = string.Empty;

    [BsonElement("respostaEscolhida")]
    public string RespostaEscolhida { get; set; } = string.Empty;

    [BsonElement("respostaCorreta")]
    public string RespostaCorreta { get; set; } = string.Empty;

    [BsonElement("correta")]
    public bool Correta { get; set; }

    [BsonElement("pontosGanhos")]
    public int PontosGanhos { get; set; }

    [BsonElement("dataResposta")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DataResposta { get; set; }
}
