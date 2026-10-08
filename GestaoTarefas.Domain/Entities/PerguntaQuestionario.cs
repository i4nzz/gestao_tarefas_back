namespace GestaoTarefas.Domain.Entities;

/// <summary>
/// Pergunta do questionário gamificado, lida da coleção "bancoQuestionario" do MongoDB.
/// </summary>
public class PerguntaQuestionario
{
    public const int PontosPadrao = 10;

    public string Id { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Dificuldade { get; set; } = string.Empty;
    public string Pergunta { get; set; } = string.Empty;
    public List<string> Opcoes { get; set; } = new();
    public int RespostaCorreta { get; set; }
    public string Explicacao { get; set; } = string.Empty;
    public int Pontos { get; set; }
    public bool Ativa { get; set; } = true;
    public int PontosPorAcerto => Pontos > 0 ? Pontos : PontosPadrao;

    public bool EstaBemFormada => Opcoes.Count >= 2 && RespostaEhValida(RespostaCorreta);

    public bool RespostaEhValida(int indice) => indice >= 0 && indice < Opcoes.Count;

    public bool EstaCorreta(int indice) => indice == RespostaCorreta;
}
