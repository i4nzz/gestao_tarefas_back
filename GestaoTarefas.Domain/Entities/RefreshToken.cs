namespace GestaoTarefas.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime DataCriacao { get; private set; }
    public DateTime DataExpiracao { get; private set; }
    public DateTime? DataRevogacao { get; private set; }
    public string? SubstituidoPorToken { get; private set; } // nota ian: evitar o reuso indevido de refresh tokens, caso o refresh token seja revogado, o novo token gerado deve ser armazenado aqui para que não seja possível reutilizar o antigo.
    protected RefreshToken() { }

    public RefreshToken(int usuarioId, string token, TimeSpan validade)
    {
        UsuarioId = usuarioId;
        Token = token;
        DataCriacao = DateTime.UtcNow;
        DataExpiracao = DateTime.UtcNow.Add(validade);
    }
    public bool EstaAtivo => DataRevogacao is null && DataExpiracao > DateTime.UtcNow;
    public void Revogar(string? substitutoPorToken = null)
    {
        DataRevogacao = DateTime.UtcNow;
        SubstituidoPorToken = substitutoPorToken;
    }
}
