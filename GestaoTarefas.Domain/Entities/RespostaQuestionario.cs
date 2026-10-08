namespace GestaoTarefas.Domain.Entities;

/// <summary>
/// Resposta dada por um filho a uma pergunta do questionário. Guarda uma cópia dos textos da
/// pergunta para o relatório continuar legível mesmo se a pergunta mudar depois.
/// </summary>
public class RespostaQuestionario
{
    public string Id { get; set; } = string.Empty;
    public int FilhoId { get; set; }
    public string PerguntaId { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Dificuldade { get; set; } = string.Empty;
    public string Pergunta { get; set; } = string.Empty;
    public string RespostaEscolhida { get; set; } = string.Empty;
    public string RespostaCorreta { get; set; } = string.Empty;
    public bool Correta { get; set; }
    public int PontosGanhos { get; set; }
    public DateTime DataResposta { get; set; } = DateTime.UtcNow;
}
