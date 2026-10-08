using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;
using GestaoTarefas.Infra.Data.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GestaoTarefas.Infra.Repositories.Mongo;

/// <summary>
/// Implementação do repositório de perguntas do questionário usando a coleção "bancoQuestionario"
/// do MongoDB, pela mesma conexão já usada para as imagens de comprovação.
/// </summary>
public class BancoQuestionarioRepository : IBancoQuestionarioRepository
{
    private readonly IMongoCollection<PerguntaQuestionarioDocumento> _colecao;

    public BancoQuestionarioRepository(IMongoDatabase database, MongoDbSettings settings)
    {
        _colecao = database.GetCollection<PerguntaQuestionarioDocumento>(settings.BancoQuestionarioCollectionName);
    }

    public async Task<IEnumerable<PerguntaQuestionario>> ObterAleatoriasAsync(int quantidade)
    {
        // $sample sorteia sem repetição dentro da mesma chamada.
        var documentos = await _colecao.Aggregate()
            .Match(d => d.Ativa)
            .Sample(quantidade)
            .ToListAsync();

        return documentos.Select(ParaEntidade);
    }

    public async Task<PerguntaQuestionario?> ObterPorIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        var documento = await _colecao.Find(d => d.Id == id).FirstOrDefaultAsync();

        return documento == null ? null : ParaEntidade(documento);
    }

    private static PerguntaQuestionario ParaEntidade(PerguntaQuestionarioDocumento documento)
    {
        return new PerguntaQuestionario
        {
            Id = documento.Id,
            Categoria = documento.Categoria,
            Dificuldade = documento.Dificuldade,
            Pergunta = documento.Pergunta,
            Opcoes = documento.Opcoes,
            RespostaCorreta = documento.RespostaCorreta,
            Explicacao = documento.Explicacao,
            Pontos = documento.Pontos,
            Ativa = documento.Ativa
        };
    }
}
