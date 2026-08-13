namespace GestaoTarefas.Application.Interfaces;

public interface IAutorizacaoFamiliarService
{
    Task<bool> PodeAcessarFilhoAsync(int filhoId);
}
