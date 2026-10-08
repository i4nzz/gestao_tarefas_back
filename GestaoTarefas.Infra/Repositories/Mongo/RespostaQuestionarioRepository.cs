using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data.Mongo;
using MongoDB.Driver;

namespace GestaoTarefas.Infra.Repositories.Mongo;

/// <summary>
/// Histórico de respostas do questionário na coleção "respostasQuestionario" do MongoDB.
/// </summary>
public class RespostaQuestionarioRepository : IRespostaQuestionarioRepository
{
    private readonly IMongoCollection<RespostaQuestionarioDocumento> _colecao;

    public RespostaQuestionarioRepository(IMongoDatabase database, MongoDbSettings settings)
    {
        _colecao = database.GetCollection<RespostaQuestionarioDocumento>(settings.RespostasQuestionarioCollectionName);
    }

    public async Task RegistrarAsync(RespostaQuestionario resposta)
    {
        var documento = new RespostaQuestionarioDocumento
        {
            FilhoId = resposta.FilhoId,
            PerguntaId = resposta.PerguntaId,
            Categoria = resposta.Categoria,
            Dificuldade = resposta.Dificuldade,
            Pergunta = resposta.Pergunta,
            RespostaEscolhida = resposta.RespostaEscolhida,
            RespostaCorreta = resposta.RespostaCorreta,
            Correta = resposta.Correta,
            PontosGanhos = resposta.PontosGanhos,
            DataResposta = resposta.DataResposta
        };

        await _colecao.InsertOneAsync(documento);
        resposta.Id = documento.Id;
    }

    public async Task<IEnumerable<RespostaQuestionario>> ObterPorFilhoAsync(int filhoId, DateTime? desde)
    {
        var filtro = Builders<RespostaQuestionarioDocumento>.Filter.Eq(d => d.FilhoId, filhoId);
        if (desde.HasValue)
        {
            filtro &= Builders<RespostaQuestionarioDocumento>.Filter.Gte(d => d.DataResposta, desde.Value);
        }

        var documentos = await _colecao.Find(filtro)
            .SortByDescending(d => d.DataResposta)
            .ToListAsync();

        return documentos.Select(d => new RespostaQuestionario
        {
            Id = d.Id,
            FilhoId = d.FilhoId,
            PerguntaId = d.PerguntaId,
            Categoria = d.Categoria,
            Dificuldade = d.Dificuldade,
            Pergunta = d.Pergunta,
            RespostaEscolhida = d.RespostaEscolhida,
            RespostaCorreta = d.RespostaCorreta,
            Correta = d.Correta,
            PontosGanhos = d.PontosGanhos,
            DataResposta = d.DataResposta
        });
    }
}
