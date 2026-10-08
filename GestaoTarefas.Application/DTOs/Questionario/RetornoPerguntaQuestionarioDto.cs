namespace GestaoTarefas.Application.DTOs.Questionario;


public class RetornoPerguntaQuestionarioDto
{
    public string Id { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Dificuldade { get; set; } = string.Empty;
    public string Pergunta { get; set; } = string.Empty;
    public List<string> Opcoes { get; set; } = new();
    public int Pontos { get; set; }
}
