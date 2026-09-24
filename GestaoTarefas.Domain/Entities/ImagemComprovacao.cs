namespace GestaoTarefas.Domain.Entities;

/// <summary>
/// Representa uma imagem de comprovação de tarefa recuperada do armazenamento (MongoDB),
/// contendo os bytes do arquivo e os metadados de quando foi salva.
/// </summary>
public class ImagemComprovacao
{
    public string Id { get; }
    public byte[] Conteudo { get; }
    public string ContentType { get; }
    public string NomeArquivoOriginal { get; }
    public DateTime DataSalvamento { get; }

    public ImagemComprovacao(string id, byte[] conteudo, string contentType, string nomeArquivoOriginal, DateTime dataSalvamento)
    {
        Id = id;
        Conteudo = conteudo;
        ContentType = contentType;
        NomeArquivoOriginal = nomeArquivoOriginal;
        DataSalvamento = dataSalvamento;
    }
}
