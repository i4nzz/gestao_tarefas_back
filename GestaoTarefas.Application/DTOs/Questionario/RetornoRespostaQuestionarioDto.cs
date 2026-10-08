namespace GestaoTarefas.Application.DTOs.Questionario;

public class RetornoRespostaQuestionarioDto
{
    public string PerguntaId { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public bool Correta { get; set; }
    public int RespostaEscolhida { get; set; }
    public int RespostaCorreta { get; set; }
    public string TextoRespostaCorreta { get; set; } = string.Empty;
    public string Explicacao { get; set; } = string.Empty;
    public int PontosGanhos { get; set; }
}
