namespace GestaoTarefas.Application.DTOs.Relatorio;

/// <summary>
/// Acompanhamento de um filho para o responsável: tarefas e desempenho no questionário.
/// </summary>
public class RelatorioFilhoDto
{
    public int FilhoId { get; set; }
    /// <summary>Período em dias considerado nos totais; nulo quando é todo o histórico.</summary>
    public int? PeriodoDias { get; set; }
    public DateTime GeradoEm { get; set; }
    public ResumoTarefasRelatorioDto Tarefas { get; set; } = new();
    public ResumoQuestionarioRelatorioDto Questionario { get; set; } = new();
}

public class ResumoTarefasRelatorioDto
{
    public int Total { get; set; }
    public int Concluidas { get; set; }
    public int AguardandoValidacao { get; set; }
    public int Pendentes { get; set; }
    public int Expiradas { get; set; }
    public int PercentualConcluidas { get; set; }
    public int ComprovacoesAprovadas { get; set; }
    public int ComprovacoesReprovadas { get; set; }
    public int PontosGanhos { get; set; }
}

public class ResumoQuestionarioRelatorioDto
{
    public int TotalRespostas { get; set; }
    public int Acertos { get; set; }
    public int Erros { get; set; }
    public int PercentualAcerto { get; set; }
    public int DiasJogados { get; set; }
    public DateTime? UltimaAtividade { get; set; }
    public List<DesempenhoCategoriaDto> PorCategoria { get; set; } = new();
    /// <summary>Sempre os últimos 7 dias (hoje incluído), independente do período escolhido.</summary>
    public List<AtividadeDiaDto> UltimosSeteDias { get; set; } = new();
    public List<RespostaRecenteDto> UltimasRespostas { get; set; } = new();
}

public class DesempenhoCategoriaDto
{
    public string Categoria { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Acertos { get; set; }
    public int PercentualAcerto { get; set; }
}

public class AtividadeDiaDto
{
    /// <summary>Data no horário de Brasília, no formato yyyy-MM-dd.</summary>
    public string Data { get; set; } = string.Empty;
    public int Acertos { get; set; }
    public int Erros { get; set; }
}

public class RespostaRecenteDto
{
    public string Pergunta { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string RespostaEscolhida { get; set; } = string.Empty;
    public string RespostaCorreta { get; set; } = string.Empty;
    public bool Correta { get; set; }
    public DateTime DataResposta { get; set; }
}
