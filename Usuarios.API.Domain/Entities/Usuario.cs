using System.Security.Cryptography;
using System.Text;
using GestaoTarefas.Domain.Enum;

namespace GestaoTarefas.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public PerfilUsuarioEnum Perfil { get; set; }
    public bool EmailConfirmado { get; private set; } = false;
    public string? TokenConfirmacaoEmail { get; private set; }
    public DateTime? TokenConfirmacaoExpiracao { get; private set; }
    public string? TokenResetSenha { get; private set; }
    public DateTime? TokenResetSenhaExpiracao { get; private set; }

    protected Usuario() { }
    public Usuario(string nome, string email, string senhaHash, PerfilUsuarioEnum perfil)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Perfil = perfil;
        Ativo = true;
    }

    public string GerarTokenConfirmacaoEmail(TimeSpan validade)
    {
        var token = Guid.NewGuid().ToString("N");
        TokenConfirmacaoEmail = token;
        TokenConfirmacaoExpiracao = DateTime.UtcNow.Add(validade);
        return token;
    }

    public bool ConfirmarEmail(string token)
    {
        if (EmailConfirmado) return true;

        if (string.IsNullOrEmpty(TokenConfirmacaoEmail) ||
            !TokensIguais(TokenConfirmacaoEmail, token) ||
            TokenConfirmacaoExpiracao is null ||
            TokenConfirmacaoExpiracao < DateTime.UtcNow)
        {
            return false;
        }

        EmailConfirmado = true;
        TokenConfirmacaoEmail = null;
        TokenConfirmacaoExpiracao = null;
        return true;
    }

    public string GerarTokenResetSenha(TimeSpan validade)
    {
        var token = Guid.NewGuid().ToString("N");
        TokenResetSenha = token;
        TokenResetSenhaExpiracao = DateTime.UtcNow.Add(validade);
        return token;
    }

    public bool ValidarTokenResetSenha(string token)
    {
        return !string.IsNullOrEmpty(TokenResetSenha)
            && TokensIguais(TokenResetSenha, token)
            && TokenResetSenhaExpiracao is not null
            && TokenResetSenhaExpiracao >= DateTime.UtcNow;
    }

    private static bool TokensIguais(string a, string b)
    {
        var bytesA = Encoding.UTF8.GetBytes(a);
        var bytesB = Encoding.UTF8.GetBytes(b);

        return bytesA.Length == bytesB.Length && CryptographicOperations.FixedTimeEquals(bytesA, bytesB);
    }

    public void RedefinirSenha(string novaSenhaHash)
    {
        SenhaHash = novaSenhaHash;
        TokenResetSenha = null;
        TokenResetSenhaExpiracao = null;
    }
}
