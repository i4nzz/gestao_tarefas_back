using System.Net;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Recompensa;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Mapping;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;

namespace GestaoTarefas.Application.Services;

public class ComprovacaoService : IComprovacaoService
{
    private readonly IComprovacaoRepository _comprovacaoRepository;
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IPontuacaoRepository _pontuacaoRepository;

    private readonly IFileStorageService _fileStorageService;
    private readonly IAutorizacaoFamiliarService _autorizacao;
    public ComprovacaoService(
        IComprovacaoRepository comprovacaoRepository
        , IPontuacaoRepository pontuacaoRepository
        , ITarefaRepository tarefaRepository
        , IFileStorageService fileStorageService
        , IAutorizacaoFamiliarService autorizacao
        )
    {
        _comprovacaoRepository = comprovacaoRepository;
        _tarefaRepository = tarefaRepository;
        _pontuacaoRepository = pontuacaoRepository;
        _fileStorageService = fileStorageService;
        _autorizacao = autorizacao;
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoComprovacaoDto>>> ObterPorTarefaAsync(int tarefaId)
    {
        var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);

        if (tarefa == null)
        {
            return new RespostaMetodos<IEnumerable<RetornoComprovacaoDto>>
            {
                Sucesso = false,
                Mensagem = "Tarefa não encontrada"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(tarefa.FilhoId))
        {
            return new RespostaMetodos<IEnumerable<RetornoComprovacaoDto>>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar as comprovações desta tarefa"
            };
        }

        var comprovacoes = await _comprovacaoRepository.ObterPorTarefaAsync(tarefaId);

        if (comprovacoes == null || !comprovacoes.Any())
        {
            return new RespostaMetodos<IEnumerable<RetornoComprovacaoDto>>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = $"Nenhuma comprovação encontrada para a tarefa {tarefaId}"
            };
        }
        var retorno = comprovacoes.ToDtoList();
        return new RespostaMetodos<IEnumerable<RetornoComprovacaoDto>>
        {
            Sucesso = true,
            StatusCode = HttpStatusCode.OK,
            ObjetoRetorno = retorno
        };
    }

    public async Task<RespostaMetodos<RetornoComprovacaoDto?>> ObterPorIdAsync(int id)
    {
        var comprovacao = await _comprovacaoRepository.ObterPorIdAsync(id);

        if (comprovacao == null)
        {
            return new RespostaMetodos<RetornoComprovacaoDto?>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Comprovação não encontrada"
            };
        }

        var tarefa = await _tarefaRepository.ObterPorIdAsync(comprovacao.TarefaId);

        if (tarefa == null || !await _autorizacao.PodeAcessarFilhoAsync(tarefa.FilhoId))
        {
            return new RespostaMetodos<RetornoComprovacaoDto?>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar esta comprovação"
            };
        }

        var retorno = comprovacao.ToDto();

        return new RespostaMetodos<RetornoComprovacaoDto?>
        {
            Sucesso = true,
            StatusCode = HttpStatusCode.OK,
            ObjetoRetorno = retorno
        };
    }

    public async Task<RespostaMetodos<(byte[] Conteudo, string ContentType)?>> ObterFotoAsync(int comprovacaoId)
    {
        var comprovacao = await _comprovacaoRepository.ObterPorIdAsync(comprovacaoId);

        if (comprovacao == null)
        {
            return new RespostaMetodos<(byte[], string)?>
            {
                Sucesso = false,
                Mensagem = "Comprovação não encontrada"
            };
        }

        var tarefa = await _tarefaRepository.ObterPorIdAsync(comprovacao.TarefaId);

        if (tarefa == null)
        {
            return new RespostaMetodos<(byte[], string)?>
            {
                Sucesso = false,
                Mensagem = "Tarefa não encontrada"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(tarefa.FilhoId))
        {
            return new RespostaMetodos<(byte[], string)?>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar esta foto"
            };
        }

        var arquivo = await _fileStorageService.ObterArquivoAsync(comprovacao.UrlFoto);

        if (arquivo == null)
        {
            return new RespostaMetodos<(byte[], string)?>
            {
                Sucesso = false,
                Mensagem = "Arquivo não encontrado no armazenamento"
            };
        }

        return new RespostaMetodos<(byte[], string)?>
        {
            Sucesso = true,
            ObjetoRetorno = arquivo
        };
    }

    public async Task<RespostaMetodos<RetornoComprovacaoDto>> EnviarAsync(CriarComprovacaoDto dto)
    {
        if (dto == null || dto.Foto == null || dto.TarefaId <= 0)
        {
            return new RespostaMetodos<RetornoComprovacaoDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Dados vazios ou inválidos"
            };
        }

        var tarefa = await _tarefaRepository.ObterPorIdAsync(dto.TarefaId);

        if (tarefa == null)
        {
            return new RespostaMetodos<RetornoComprovacaoDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Tarefa não encontrada"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(tarefa.FilhoId))
        {
            return new RespostaMetodos<RetornoComprovacaoDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para enviar comprovação para esta tarefa"
            };
        }

        string caminhoArquivo = string.Empty;

        try
        {
            caminhoArquivo = await _fileStorageService.SalvarArquivoAsync(dto.Foto, "Comprovacoes");
        }

        catch (ArgumentException ex)
        {
            return new RespostaMetodos<RetornoComprovacaoDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = ex.Message
            };
        }

        var comprovacao = new ComprovacaoTarefa(dto.TarefaId, caminhoArquivo);

        await _comprovacaoRepository.AdicionarAsync(comprovacao);

        var retornoComprovacao = comprovacao.ToDto();

        return new RespostaMetodos<RetornoComprovacaoDto>
        {
            Sucesso = true,
            ObjetoRetorno = retornoComprovacao
        };
    }

    public async Task<RespostaMetodos<RetornoComprovacaoDto>> ValidarAsync(int id, bool aprovar)
    {
        var comprovacao = await _comprovacaoRepository.ObterPorIdAsync(id);

        if (comprovacao == null)
        {
            return new RespostaMetodos<RetornoComprovacaoDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Comprovação não encontrada"
            };
        }

        var retornoTarefa = await _tarefaRepository.ObterPorIdAsync(comprovacao.TarefaId);

        if (retornoTarefa == null)
        {
            return new RespostaMetodos<RetornoComprovacaoDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Tarefa para essa validação não foi encontrada"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(retornoTarefa.FilhoId))
        {
            return new RespostaMetodos<RetornoComprovacaoDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para validar esta comprovação"
            };
        }

        if (aprovar)
        {
            if (comprovacao.Status == StatusValidacaoTarefaEnum.Aprovada)
            {
                return new RespostaMetodos<RetornoComprovacaoDto>
                {
                    Sucesso = true,
                    ObjetoRetorno = null,
                    Mensagem = "Comprovação já estava aprovada"
                };
            }

            comprovacao.Aprovar();

            var existePontos = await _pontuacaoRepository.ExisteAsync(retornoTarefa.TarefaId, retornoTarefa.FilhoId);

            if (!existePontos)
            {
                var pontuacao = Pontuacao.CriarGanho(retornoTarefa.FilhoId, retornoTarefa.TarefaId, retornoTarefa.Pontos);
                await _pontuacaoRepository.AdicionarAsync(pontuacao);
            }
        }
        else
        {
            if (comprovacao.Status == StatusValidacaoTarefaEnum.Reprovada)
            {
                return new RespostaMetodos<RetornoComprovacaoDto>
                {
                    Sucesso = true,
                    ObjetoRetorno = null,
                    Mensagem = "Comprovação já estava reprovada"
                };
            }

            if (comprovacao.Status == StatusValidacaoTarefaEnum.Aprovada)
            {
                return new RespostaMetodos<RetornoComprovacaoDto>
                {
                    Sucesso = false,
                    Mensagem = "Comprovação já foi aprovada e não pode ser reprovada"
                };
            }

            comprovacao.Reprovar();
        }
        await _comprovacaoRepository.AtualizarAsync(comprovacao);
        var retornoComprovacao = comprovacao.ToDto();

        return new RespostaMetodos<RetornoComprovacaoDto>
        {
            Sucesso = true,
            ObjetoRetorno = retornoComprovacao,
            Mensagem = aprovar ? "Comprovação aprovada com sucesso" : "Comprovação reprovada com sucesso"
        };
    }
}