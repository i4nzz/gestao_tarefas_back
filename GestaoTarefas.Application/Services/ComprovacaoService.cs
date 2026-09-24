using System.Net;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Recompensa;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Mapping;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using GestaoTarefas.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GestaoTarefas.Application.Services;

public class ComprovacaoService : IComprovacaoService
{
    private static readonly string[] ExtensoesPermitidas = { ".jpg", ".jpeg", ".png" };
    private const long TamanhoMaximoBytes = 5 * 1024 * 1024; // 5MB

    private readonly IComprovacaoRepository _comprovacaoRepository;
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IPontuacaoRepository _pontuacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;

    private readonly IImagemComprovacaoRepository _imagemComprovacaoRepository;
    private readonly IAutorizacaoFamiliarService _autorizacao;
    public ComprovacaoService(
        IComprovacaoRepository comprovacaoRepository
        , IPontuacaoRepository pontuacaoRepository
        , ITarefaRepository tarefaRepository
        , IUsuarioRepository usuarioRepository
        , IEmailService emailService
        , IImagemComprovacaoRepository imagemComprovacaoRepository
        , IAutorizacaoFamiliarService autorizacao
        )
    {
        _comprovacaoRepository = comprovacaoRepository;
        _tarefaRepository = tarefaRepository;
        _pontuacaoRepository = pontuacaoRepository;
        _usuarioRepository = usuarioRepository;
        _emailService = emailService;
        _imagemComprovacaoRepository = imagemComprovacaoRepository;
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

        var imagem = await _imagemComprovacaoRepository.ObterPorIdAsync(comprovacao.ImagemId);

        if (imagem == null)
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
            ObjetoRetorno = (imagem.Conteudo, imagem.ContentType)
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

        var erroValidacao = ValidarFoto(dto.Foto);

        if (erroValidacao != null)
        {
            return new RespostaMetodos<RetornoComprovacaoDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = erroValidacao
            };
        }

        byte[] conteudoFoto;

        using (var memoryStream = new MemoryStream())
        {
            await dto.Foto.CopyToAsync(memoryStream);
            conteudoFoto = memoryStream.ToArray();
        }

        var comprovacaoExistente = await _comprovacaoRepository.ObterUltimaPorTarefaAsync(dto.TarefaId);

        ComprovacaoTarefa comprovacao;

        if (comprovacaoExistente != null)
        {
            if (comprovacaoExistente.Status != StatusValidacaoTarefaEnum.Pendente)
            {
                return new RespostaMetodos<RetornoComprovacaoDto>
                {
                    Sucesso = false,
                    ObjetoRetorno = null,
                    Mensagem = "Esta tarefa já possui uma comprovação validada (aprovada ou reprovada) e não permite o envio de uma nova foto."
                };
            }

            await _imagemComprovacaoRepository.AtualizarAsync(comprovacaoExistente.ImagemId, conteudoFoto, dto.Foto.ContentType, dto.Foto.FileName);

            comprovacaoExistente.SubstituirFoto();

            await _comprovacaoRepository.AtualizarAsync(comprovacaoExistente);

            comprovacao = comprovacaoExistente;
        }
        else
        {
            var imagemId = await _imagemComprovacaoRepository.SalvarAsync(conteudoFoto, dto.Foto.ContentType, dto.Foto.FileName);

            comprovacao = new ComprovacaoTarefa(dto.TarefaId, imagemId);

            await _comprovacaoRepository.AdicionarAsync(comprovacao);
        }

        await NotificarPaisAsync(tarefa);

        var retornoComprovacao = comprovacao.ToDto();

        return new RespostaMetodos<RetornoComprovacaoDto>
        {
            Sucesso = true,
            ObjetoRetorno = retornoComprovacao
        };
    }

    private static string? ValidarFoto(IFormFile foto)
    {
        if (foto.Length == 0)
        {
            return "Arquivo inválido ou vazio.";
        }

        if (foto.Length > TamanhoMaximoBytes)
        {
            return "Arquivo excede o tamanho máximo permitido (5MB).";
        }

        var extensao = Path.GetExtension(foto.FileName).ToLowerInvariant();

        if (!ExtensoesPermitidas.Contains(extensao))
        {
            return "Tipo de arquivo não permitido. Use JPG ou PNG.";
        }

        return null;
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

    private async Task NotificarPaisAsync(Tarefa tarefa)
    {
        var pais = await _usuarioRepository.ObterPaisPorFilhoIdAsync(tarefa.FilhoId);
        var nomeFilho = tarefa.Filho?.Nome ?? "Seu filho";

        foreach (var pai in pais)
        {
            await _emailService.EnviarNotificacaoSistemaAsync(
                pai.Email,
                "Nova tarefa aguardando validação",
                $"{nomeFilho} enviou uma comprovação para a tarefa \"{tarefa.Titulo}\" e ela está aguardando sua validação no Task Kids.");
        }
    }
}