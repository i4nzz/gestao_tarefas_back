namespace GestaoTarefas.Application.DTOs.Pontuacao;

public class RetornoPontuacaoDto
{
    public int Id { get; set; }
    public int FilhoId { get; set; }
    public int TarefaId { get; set; }
    public string TituloTarefa { get; set; } = string.Empty;
    public int Pontos { get; set; }
    public DateTime DataRegistro { get; set; }
}
